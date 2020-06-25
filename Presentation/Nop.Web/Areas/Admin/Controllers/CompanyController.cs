using System;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Discounts;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.ExportImport;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Core.Infrastructure;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class CompanyController : BaseAdminController
    {
        #region Fields

        private readonly IAclService _aclService;
        private readonly ICompanyModelFactory _companyModelFactory;
        private readonly ICompanyService _companyService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICustomerService _customerService;
        private readonly IDiscountService _discountService;
        private readonly IExportManager _exportManager;
        private readonly IImportManager _importManager;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly IPictureService _pictureService;
        private readonly IProductService _productService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IStoreService _storeService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public CompanyController(IAclService aclService,
            ICompanyModelFactory companyModelFactory,
            ICompanyService companyService,
            ICustomerActivityService customerActivityService,
            ICustomerService customerService,
            IDiscountService discountService,
            IExportManager exportManager,
            IImportManager importManager,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            INotificationService notificationService,
            IPermissionService permissionService,
            IPictureService pictureService,
            IProductService productService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IUrlRecordService urlRecordService,
            IWorkContext workContext)
        {
            _aclService = aclService;
            _companyModelFactory = companyModelFactory;
            _companyService = companyService;
            _customerActivityService = customerActivityService;
            _customerService = customerService;
            _discountService = discountService;
            _exportManager = exportManager;
            _importManager = importManager;
            _localizationService = localizationService;
            _localizedEntityService = localizedEntityService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _pictureService = pictureService;
            _productService = productService;
            _storeMappingService = storeMappingService;
            _storeService = storeService;
            _urlRecordService = urlRecordService;
            _workContext = workContext;
        }

        #endregion
        #region List

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            //prepare model
            var model = _companyModelFactory.PrepareCompanySearchModel(new CompanySearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult List(CompanySearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _companyModelFactory.PrepareCompanyListModel(searchModel);

            return Json(model);
        }

        #endregion

        #region Create / Edit / Delete

        public virtual IActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            //prepare model
            var model = _companyModelFactory.PrepareCompanyModel(new CompanyModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(CompanyModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var company = model.ToEntity<Company>();
                company.CreatedOnUtc = DateTime.UtcNow;
                company.UpdatedOnUtc = DateTime.UtcNow;
                _companyService.InsertCompany(company);

                //_categoryService.UpdateCategory(category);

                //activity log
                _customerActivityService.InsertActivity("AddNewCompany",
                    string.Format(_localizationService.GetResource("ActivityLog.AddNewCompany"), company.Name), company);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Catalog.Companies.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = company.Id });
            }

            //prepare model
            model = _companyModelFactory.PrepareCompanyModel(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            //try to get a category with the specified id
            var company = _companyService.GetCompanyById(id);
            if (company == null || company.Deleted)
                return RedirectToAction("List");

            //prepare model
            var model = _companyModelFactory.PrepareCompanyModel(null, company);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(CompanyModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            //try to get a category with the specified id
            var company = _companyService.GetCompanyById(model.Id);
            if (company == null || company.Deleted)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                var prevPictureId = company.PictureId;

                company = model.ToEntity(company);
                company.UpdatedOnUtc = DateTime.UtcNow;
                _companyService.UpdateCompany(company);

                //delete an old picture (if deleted or updated)
                if (prevPictureId > 0 && prevPictureId != company.PictureId)
                {
                    var prevPicture = _pictureService.GetPictureById(prevPictureId);
                    if (prevPicture != null)
                        _pictureService.DeletePicture(prevPicture);
                }

                //activity log
                _customerActivityService.InsertActivity("EditCompany",
                    string.Format(_localizationService.GetResource("ActivityLog.EditCompany"), company.Name), company);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Catalog.Companies.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = company.Id });
            }

            //prepare model
            model = _companyModelFactory.PrepareCompanyModel(model, company, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            //try to get a category with the specified id
            var company = _companyService.GetCompanyById(id);
            if (company == null)
                return RedirectToAction("List");

            _companyService.DeleteCompany(company);

            //activity log
            _customerActivityService.InsertActivity("DeleteCompany",
                string.Format(_localizationService.GetResource("ActivityLog.DeleteCompany"), company.Name), company);

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Catalog.Companies.Deleted"));

            return RedirectToAction("List");
        }

        #endregion
    }
}
