            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adjusting stock");
                TempData["ErrorMessage"] = "Error adjusting stock. Please try again.";
                return View(model);
            }
        }
    }
}using inventory_app.Pages.Model;
using Microsoft.AspNetCore.Mvc;

namespace inventory_app.Pages.InventoryController
{
    public class InventoryController : Controller
    {
        private readonly IGetStockUseCase _getStockUseCase;
        private readonly IStockAdjustmentUseCase _stockAdjustmentUseCase;
        private readonly ILogger<InventoryController> _logger;

        private string CurrentUserId =>
            User.Identity?.Name ?? "system";

        public InventoryController(
            IGetStockUseCase getStockUseCase,
            IStockAdjustmentUseCase stockAdjustmentUseCase,
            ILogger<InventoryController> logger)
        {
            _getStockUseCase = getStockUseCase;
            _stockAdjustmentUseCase = stockAdjustmentUseCase;
            _logger = logger;
        }

        public async Task<IActionResult> Index(
            string? searchTerm,
            bool showSummary = false)
        {
            try
            {
                var model = new InventoryViewModel
                {
                    SearchTerm = searchTerm,
                    ShowSummary = showSummary
                };

                if (showSummary)
                {
                    var summaryResult =
                        await _getStockUseCase.GetStockSummaryAsync();

                    if (summaryResult.IsSuccess)
                    {
                        model.StockSummary = summaryResult.Value
                            .OrderBy(s => s.ItemSku)
                            .ToList();
                    }
                }
                else
                {
                    var result =
                        await _getStockUseCase.GetAllStockAsync();

                    if (result.IsSuccess)
                    {
                        var stockItems = result.Value
                            .Where(s => s.AvailableQuantity > 0);

                        if (!string.IsNullOrWhiteSpace(searchTerm))
                        {
                            stockItems = stockItems.Where(s =>
                                s.ItemSku.Contains(
                                    searchTerm,
                                    StringComparison.OrdinalIgnoreCase) ||
                                s.ItemName.Contains(
                                    searchTerm,
                                    StringComparison.OrdinalIgnoreCase) ||
                                s.LocationCode.Contains(
                                    searchTerm,
                                    StringComparison.OrdinalIgnoreCase));
                        }

                        model.StockItems = stockItems.ToList();
                    }
                }

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading inventory data");
                TempData["ErrorMessage"] =
                    "Error loading inventory data. Please try again.";

                return View(new InventoryViewModel());
            }
        }

        [HttpGet]
        public IActionResult Adjust(
            string itemSku,
            string locationCode,
            decimal currentQuantity)
        {
            var model = new StockAdjustmentViewModel
            {
                ItemSku = itemSku,
                LocationCode = locationCode,
                CurrentQuantity = currentQuantity,
                NewQuantity = currentQuantity
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Adjust(
            StockAdjustmentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var request = new StockAdjustmentDto(
                    model.ItemSku,
                    model.LocationCode,
                    model.NewQuantity,
                    model.Reason);

                var result = await _stockAdjustmentUseCase.ExecuteAsync(
                    request,
                    CurrentUserId);

                if (result.IsFailure)
                {
                    TempData["ErrorMessage"] = result.Error;
                    return View(model);
                }

                TempData["SuccessMessage"] =
                    $"Stock adjusted successfully! Movement ID: " +
                    $"{result.Value.MovementId}";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adjusting stock");
                TempData["ErrorMessage"] =
                    "Error adjusting stock. Please try again.";

                return View(model);
            }
        }
    }
}using inventory_app.Pages.Model;
using Microsoft.AspNetCore.Mvc;

namespace inventory_app.Pages.InventoryController
{
    public class InventoryController : Controller
    {
        private readonly IGetStockUseCase _getStockUseCase;
        private readonly IStockAdjustmentUseCase _stockAdjustmentUseCase;
        private readonly ILogger<InventoryController> _logger;

        private string CurrentUserId =>
            User.Identity?.Name ?? "system";

        public InventoryController(
            IGetStockUseCase getStockUseCase,
            IStockAdjustmentUseCase stockAdjustmentUseCase,
            ILogger<InventoryController> logger)
        {
            _getStockUseCase = getStockUseCase;
            _stockAdjustmentUseCase = stockAdjustmentUseCase;
            _logger = logger;
        }

        public async Task<IActionResult> Index(
            string? searchTerm,
            bool showSummary = false)
        {
            try
            {
                var model = new InventoryViewModel
                {
                    SearchTerm = searchTerm,
                    ShowSummary = showSummary
                };

                if (showSummary)
                {
                    var summaryResult =
                        await _getStockUseCase.GetStockSummaryAsync();

                    if (summaryResult.IsSuccess)
                    {
                        model.StockSummary = summaryResult.Value
                            .OrderBy(s => s.ItemSku)
                            .ToList();
                    }
                }
                else
                {
                    var result =
                        await _getStockUseCase.GetAllStockAsync();

                    if (result.IsSuccess)
                    {
                        var stockItems = result.Value
                            .Where(s => s.AvailableQuantity > 0);

                        if (!string.IsNullOrWhiteSpace(searchTerm))
                        {
                            stockItems = stockItems.Where(s =>
                                s.ItemSku.Contains(
                                    searchTerm,
                                    StringComparison.OrdinalIgnoreCase) ||
                                s.ItemName.Contains(
                                    searchTerm,
                                    StringComparison.OrdinalIgnoreCase) ||
                                s.LocationCode.Contains(
                                    searchTerm,
                                    StringComparison.OrdinalIgnoreCase));
                        }

                        model.StockItems = stockItems.ToList();
                    }
                }

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading inventory data");
                TempData["ErrorMessage"] =
                    "Error loading inventory data. Please try again.";

                return View(new InventoryViewModel());
            }
        }

        [HttpGet]
        public IActionResult Adjust(
            string itemSku,
            string locationCode,
            decimal currentQuantity)
        {
            var model = new StockAdjustmentViewModel
            {
                ItemSku = itemSku,
                LocationCode = locationCode,
                CurrentQuantity = currentQuantity,
                NewQuantity = currentQuantity
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Adjust(
            StockAdjustmentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var request = new StockAdjustmentDto(
                    model.ItemSku,
                    model.LocationCode,
                    model.NewQuantity,
                    model.Reason);

                var result = await _stockAdjustmentUseCase.ExecuteAsync(
                    request,
                    CurrentUserId);

                if (result.IsFailure)
                {
                    TempData["ErrorMessage"] = result.Error;
                    return View(model);
                }

                TempData["SuccessMessage"] =
                    $"Stock adjusted succesusing inventory_app.Pages.Model;
using Microsoft.AspNetCore.Mvc;

namespace inventory_app.Pages.InventoryController
{
    public class InventoryController : Controller
    {
        private readonly IGetStockUseCase _getStockUseCase;
        private readonly IStockAdjustmentUseCase _stockAdjustmentUseCase;
        private readonly ILogger<InventoryController> _logger;

        private string CurrentUserId =>
            User.Identity?.Name ?? "system";

        public InventoryController(
            IGetStockUseCase getStockUseCase,
            IStockAdjustmentUseCase stockAdjustmentUseCase,
            ILogger<InventoryController> logger)
        {
            _getStockUseCase = getStockUseCase;
            _stockAdjustmentUseCase = stockAdjustmentUseCase;
            _logger = logger;
        }

        public async Task<IActionResult> Index(
            string? searchTerm,
            bool showSummary = false)
        {
            try
            {
                var model = new InventoryViewModel
                {
                    SearchTerm = searchTerm,
                    ShowSummary = showSummary
                };

                if (showSummary)
                {
                    var summaryResult =
                        await _getStockUseCase.GetStockSummaryAsync();

                    if (summaryResult.IsSuccess)
                    {
                        model.StockSummary = summaryResult.Value
                            .OrderBy(s => s.ItemSku)
                            .ToList();
                    }
                }
                else
                {
                    var result =
                        await _getStockUseCase.GetAllStockAsync();

                    if (result.IsSuccess)
                    {
                        var stockItems = result.Value
                            .Where(s => s.AvailableQuantity > 0);

                        if (!string.IsNullOrWhiteSpace(searchTerm))
                        {
                            stockItems = stockItems.Where(s =>
                                s.ItemSku.Contains(
                                    searchTerm,
                                    StringComparison.OrdinalIgnoreCase) ||
                                s.ItemName.Contains(
                                    searchTerm,
                                    StringComparison.OrdinalIgnoreCase) ||
                                s.LocationCode.Contains(
                                    searchTerm,
                                    StringComparison.OrdinalIgnoreCase));
                        }

                        model.StockItems = stockItems.ToList();
                    }
                }

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading inventory data");
                TempData["ErrorMessage"] =
                    "Error loading inventory data. Please try again.";

                return View(new InventoryViewModel());
            }
        }

        [HttpGet]
        public IActionResult Adjust(
            string itemSku,
            string locationCode,
            decimal currentQuantity)
        {
            var model = new StockAdjustmentViewModel
            {
                ItemSku = itemSku,
                LocationCode = locationCode,
                CurrentQuantity = currentQuantity,
                NewQuantity = currentQuantity
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Adjust(
            StockAdjustmentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var request = new StockAdjustmentDto(
                    model.ItemSku,
                    model.LocationCode,
                    model.NewQuantity,
                    model.Reason);

                var result = await _stockAdjustmentUseCase.ExecuteAsync(
                    request,
                    CurrentUserId);

                if (result.IsFailure)
                {
                    TempData["ErrorMessage"] = result.Error;
                    return View(model);
                }

                TempData["SuccessMessage"] =
                    $"Stock adjusted successfully! Movement ID: " +
                    $"{result.Value.MovementId}";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adjusting stock");
                TempData["ErrorMessage"] =
                    "Error adjusting stock. Please try again.";

                return View(model);
            }
        }
    }
}using inventory_app.Pages.Model;
using Microsoft.AspNetCore.Mvc;

namespace inventory_app.Pages.InventoryController
{
    public class InventoryController : Controller
    {
        private readonly IGetStockUseCase _getStockUseCase;
        private readonly IStockAdjustmentUseCase _stockAdjustmentUseCase;
        private readonly ILogger<InventoryController> _logger;

        private string CurrentUserId =>
            User.Identity?.Name ?? "system";

        public InventoryController(
            IGetStockUseCase getStockUseCase,
            IStockAdjustmentUseCase stockAdjustmentUseCase,
            ILogger<InventoryController> logger)
        {
            _getStockUseCase = getStockUseCase;
            _stockAdjustmentUseCase = stockAdjustmentUseCase;
            _logger = logger;
        }

        public async Task<IActionResult> Index(
            string? searchTerm,
            bool showSummary = false)
        {
            try
            {
                var model = new InventoryViewModel
                {
                    SearchTerm = searchTerm,
                    ShowSummary = showSummary
                };

                if (showSummary)
                {
                    var summaryResult =
                        await _getStockUseCase.GetStockSummaryAsync();

                    if (summaryResult.IsSuccess)
                    {
                        model.StockSummary = summaryResult.Value
                            .OrderBy(s => s.ItemSku)
                            .ToList();
                    }
                }
                else
                {
                    var result =
                        await _getStockUseCase.GetAllStockAsync();

                    if (result.IsSuccess)
                    {
                        var stockItems = result.Value
                            .Where(s => s.AvailableQuantity > 0);

                        if (!string.IsNullOrWhiteSpace(searchTerm))
                        {
                            stockItems = stockItems.Where(s =>
                                s.ItemSku.Contains(
                                    searchTerm,
                                    StringComparison.OrdinalIgnoreCase) ||
                                s.ItemName.Contains(
                                    searchTerm,
                                    StringComparison.OrdinalIgnoreCase) ||
                                s.LocationCode.Contains(
                                    searchTerm,
                                    StringComparison.OrdinalIgnoreCase));
                        }

                        model.StockItems = stockItems.ToList();
                    }
                }

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading inventory data");
                TempData["ErrorMessage"] =
                    "Error loading inventory data. Please try again.";

                return View(new InventoryViewModel());
            }
        }

        [HttpGet]
        public IActionResult Adjust(
            string itemSku,
            string locationCode,
            decimal currentQuantity)
        {
            var model = new StockAdjustmentViewModel
            {
                ItemSku = itemSku,
                LocationCode = locationCode,
                CurrentQuantity = currentQuantity,
                NewQuantity = currentQuantity
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Adjust(
            StockAdjustmentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var request = new StockAdjustmentDto(
                    model.ItemSku,
                    model.LocationCode,
                    model.NewQuantity,
                    model.Reason);

                var result = await _stockAdjustmentUseCase.ExecuteAsync(
                    request,
                    CurrentUserId);

                if (result.IsFailure)
                {
                    TempData["ErrorMessage"] = result.Error;
                    return View(model);
                }

                TempData["SuccessMessage"] =
                    $"Stock adjusted successfully! Movement ID: " +
                    $"{result.Value.MovementId}";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adjusting stock");
                TempData["ErrorMessage"] =
                    "Error adjusting stock. Please try again.";

                return View(model);
            }
        }
    }
}$path = 'C:\Users\j\source\repos\inventory app\Pages\Controller\InventoryController.cs'
$lines = Get-Content -LiteralPath $path
$lines[9..155] | Set-Content -LiteralPath $path -Encoding utf8sfully! Movement ID: " +
                    $"{result.Value.MovementId}";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adjusting stock");
                TempData["ErrorMessage"] =
                    "Error adjusting stock. Please try again.";

                return View(model);
            }
        }
    }
}