using Nop.Core;
using Nop.Core.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Services.Catalog
{
    public partial interface ICompanyService
    {
        /// <summary>
        /// Delete Company
        /// </summary>
        /// <param name="company">Company</param>
        void DeleteCompany(Company company);

        /// <summary>
        /// Gets all Company
        /// </summary>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <param name="loadCacheableCopy">A value indicating whether to load a copy that could be cached (workaround until Entity Framework supports 2-level caching)</param>
        /// <returns>Categories</returns>
        IList<Company> GetAllCompanies(int storeId = 0, bool showHidden = false, bool loadCacheableCopy = true);

        /// <summary>
        /// Gets all Company
        /// </summary>
        /// <param name="companyName">Category name</param>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Categories</returns>
        IPagedList<Company> GetAllCompanies(string companyName, int storeId = 0,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);

        /// <summary>
        /// Gets a category
        /// </summary>
        /// <param name="companyId">Category identifier</param>
        /// <returns>Category</returns>
        Company GetCompanyById(int companyId);

        /// <summary>
        /// Inserts category
        /// </summary>
        /// <param name="company">Category</param>
        void InsertCompany(Company company);

        /// <summary>
        /// Updates the category
        /// </summary>
        /// <param name="company">Company</param>
        void UpdateCompany(Company company);
    }
}
