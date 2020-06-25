using Nop.Core.Domain.Catalog;
using Nop.Web.Areas.Admin.Models.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Areas.Admin.Factories
{
    public partial interface ICompanyModelFactory
    {
        CompanySearchModel PrepareCompanySearchModel(CompanySearchModel searchModel);

        CompanyListModel PrepareCompanyListModel(CompanySearchModel searchModel);

        CompanyModel PrepareCompanyModel(CompanyModel model, Company company, 
            bool excludeProperties = false);

    }
}
