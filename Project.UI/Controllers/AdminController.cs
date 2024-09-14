using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.Business.Abstract;
using Project.UI.Entities;
using System.Security.Claims;
using System.Linq;
using Project.Entities.Concrete;
using Project.UI.Models;
using Microsoft.AspNetCore.Hosting;
using Project.UI.Helpers;
using System.Threading.Tasks;
using Project.UI.Statics;
using Project.UI.Services;
using System.Collections.Generic;

namespace Project.UI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private IProductService _productService;
        private IModelService _modelService;
        private IMaterialService _materialService;
        private IOrderStatusService _orderStatusService;
        private IComboService _comboService;
        private IUserService _userService;
        private readonly ComboProductsHelper _chelper;
        private readonly ICartSessionService _carthelper;
        private readonly IOrderSessionService _orderSessionService;
        private ProductModifyHelper _helper;
        private User user;
        public AdminController(IWebHostEnvironment _webHost, ICartSessionService CartService, IProductService productService, IComboService comboService, IModelService modelService, IUserService userService, IMaterialService materialService, IOrderSessionService orderSessionService, IOrderStatusService orderStatusService)
        {
            _productService = productService;
            _comboService = comboService;
            _modelService = modelService;
            _carthelper = CartService;
            _chelper = new ComboProductsHelper(_productService, _modelService, _carthelper, materialService);
            _userService = userService;
            _materialService = materialService;
            _helper = new ProductModifyHelper(_webHost, _productService, _modelService, _materialService, CartService);
            _orderSessionService = orderSessionService;
            _orderStatusService = orderStatusService;
        }
        List<Model> Models { get; set; }
        public async Task<IActionResult> Index()
        {
            user = Static.UserStart(this, _userService);
            var vm = await _helper.Product();
            vm.User = user;
            return View(vm);
        }
        public async Task<IActionResult> Product(int modelId = 0, int materialId = 0)
        {
            user = Static.UserStart(this, _userService);
            var vm = await _helper.Product(modelId,materialId);
            vm.User = user;
            return View(vm);
        }
        public async Task<IActionResult> Model()
        {
            user = Static.UserStart(this, _userService);
            var vm = await _helper.Product();
            vm.User = user;
            return View(vm);
        }
        public async Task<IActionResult> Material()
        {
            user = Static.UserStart(this, _userService);
            var vm = await _helper.Product();
            vm.User = user;
            return View(vm);
        }
        public async Task<IActionResult> Orders()
        {
            user = Static.UserStart(this, _userService);
            var vm = _orderSessionService.GetOrders();
            vm.User = user;
            return View(vm);
        }
        public IActionResult DeleteProduct(int productId)
        {
            user = Static.UserStart(this, _userService);
            _productService.RemoveProduct(productId);
            return RedirectToAction("Product", "Admin");
        }
        public IActionResult DeleteModel(int modelId)
        {
            user = Static.UserStart(this, _userService);
            _modelService.RemoveModel(modelId);
            return RedirectToAction("Model", "Admin");
        }
        public IActionResult DeleteMaterial(int materialId)
        {
            user = Static.UserStart(this, _userService);
            _materialService.RemoveMaterial(materialId);
            return RedirectToAction("Material", "Admin");
        }
        public IActionResult UpdateProduct(int productId)
        {
            user = Static.UserStart(this, _userService);
            var product = _productService.GetProduct(productId);
            var model = new ProductModifyModel
            {
                Product = product,
                User = user,
                Materials = _materialService.GetMaterials(),
                Models = _modelService.GetModels(),
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateProductAsync(ProductModifyModel productModifyModel)
        {
            user = Static.UserStart(this, _userService);
            var updateProduct = await _helper.ProductCreation(productModifyModel);
            _productService.UpdateProduct(updateProduct);
            return RedirectToAction("Product", "Admin");
        }
        public IActionResult AddProduct()
        {
            user = Static.UserStart(this, _userService);
            var model = new ProductModifyModel
            {
                Product = new Product(),
                User = user,
                Materials = _materialService.GetMaterials(),
                Models = _modelService.GetModels(),
            };
            return View(model);
        }


        public IActionResult UpdateOrder(Order order, int Status)
        {
            user = Static.UserStart(this, _userService);
            _orderSessionService.UpdateOrder(order.Id, Status);
            return RedirectToAction("Orders", "Admin");
        }
        [HttpPost]
        public async Task<IActionResult> AddProductAsync(ProductModifyModel productModifyModel)
        {
            user = Static.UserStart(this, _userService);
            var addProduct = await _helper.ProductCreation(productModifyModel);
            _productService.AddProduct(addProduct);
            return RedirectToAction("Product", "Admin");
        }
        public IActionResult AddModel()
        {
            user = Static.UserStart(this, _userService);
            var model = new ModelViewModel
            {
                Model = new Model(),
                User = user,
                Materials = _materialService.GetMaterials(),
                Models = _modelService.GetModels(),
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> AddModelAsync(ModelViewModel modelViewModel)
        {
            var filepath = await _helper.ImageHelper(modelViewModel.File);
            modelViewModel.Model.ImagePath = filepath;
            _modelService.AddModel(modelViewModel.Model);
            return RedirectToAction("Model", "Admin");
        }
        public IActionResult AddMaterial()
        {
            user = Static.UserStart(this, _userService);
            var model = new MaterialViewModel
            {
                Material = new Material(),
                User = user,
                Materials = _materialService.GetMaterials(),
                Models = _modelService.GetModels(),
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> AddMaterialAsync(MaterialViewModel materialViewModel)
        {
            var filepath = await _helper.ImageHelper(materialViewModel.File);
            materialViewModel.Material.ImagePath = filepath;
            _materialService.AddMaterial(materialViewModel.Material);
            return RedirectToAction("Material", "Admin");

        }

        public IActionResult UpdateMaterial(int materialId)
        {
            user = Static.UserStart(this, _userService);
            var model = new MaterialViewModel
            {
                Material = _materialService.GetMaterial(materialId),
                User = user,
                Materials = _materialService.GetMaterials(),
                Models = _modelService.GetModels(),
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateMaterialAsync(MaterialViewModel materialViewModel)
        {
            string filepath = materialViewModel.Material.ImagePath;
            if (materialViewModel.File != null)
            {
                filepath = await _helper.ImageHelper(materialViewModel.File);
            }
            materialViewModel.Material.ImagePath = filepath;
            _materialService.UpdateMaterial(materialViewModel.Material);
            return RedirectToAction("Material", "Admin");

        }



        public IActionResult UpdateModel(int modelId)
        {
            user = Static.UserStart(this, _userService);
            var model = new ModelViewModel
            {
                Model = _modelService.GetModel(modelId),
                User = user,
                Materials = _materialService.GetMaterials(),
                Models = _modelService.GetModels(),
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateModelAsync(ModelViewModel modelViewModel)
        {
            string filepath = modelViewModel.Model.ImagePath;
            if (modelViewModel.File != null)
            {
                filepath = await _helper.ImageHelper(modelViewModel.File);
            }
            modelViewModel.Model.ImagePath = filepath;
            _modelService.UpdateModel(modelViewModel.Model);
            return RedirectToAction("Model", "Admin");

        }

    }
}
