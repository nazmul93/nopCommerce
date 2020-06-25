using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Areas.Admin.Models.Catalog
{
    public class CompanySearchModel : BaseSearchModel
    {
        #region Ctor

        public CompanySearchModel()
        {
            AvailableCompanies = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        [NopResourceDisplayName("Admin.Catalog.Companies.List.SearchCompanyName")]
        public string SearchCompanyName { get; set; }

        public IList<SelectListItem> AvailableCompanies { get; set; }

        #endregion
    }
}
