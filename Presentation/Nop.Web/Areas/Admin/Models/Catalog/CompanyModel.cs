using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Areas.Admin.Models.Catalog
{
    public partial class CompanyModel : BaseNopEntityModel, ILocalizedModel<CategoryLocalizedModel>
    {
        #region Ctor

        public CompanyModel()
        {
            if (PageSize < 1)
            {
                PageSize = 5;
            }

            Locales = new List<CategoryLocalizedModel>();
        }

        #endregion

        #region Properties

        [NopResourceDisplayName("Admin.Catalog.Companies.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Companies.Fields.Description")]
        public string Description { get; set; }

        [UIHint("Picture")]
        [NopResourceDisplayName("Admin.Catalog.Companies.Fields.Picture")]
        public int PictureId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Companies.Fields.PageSize")]
        public int PageSize { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Companies.Fields.Published")]
        public bool Published { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Companies.Fields.Deleted")]
        public bool Deleted { get; set; }

        public IList<CategoryLocalizedModel> Locales { get; set; }

        #endregion
    }

    public partial class CompanyLocalizedModel : ILocalizedLocaleModel
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Companies.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Companies.Fields.Description")]
        public string Description { get; set; }
    }
}
