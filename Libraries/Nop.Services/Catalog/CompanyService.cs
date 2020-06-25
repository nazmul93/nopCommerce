using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Data.Extensions;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Stores;
using Nop.Data;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Stores;

namespace Nop.Services.Catalog
{
    public partial class CompanyService : ICompanyService
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly CommonSettings _commonSettings;
        private readonly IAclService _aclService;
        private readonly ICacheManager _cacheManager;
        private readonly IDataProvider _dataProvider;
        private readonly IDbContext _dbContext;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILocalizationService _localizationService;
        private readonly IRepository<AclRecord> _aclRepository;
        private readonly IRepository<Company> _companyRepository;
        private readonly IRepository<StoreMapping> _storeMappingRepository;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreContext _storeContext;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public CompanyService(CatalogSettings catalogSettings,
            CommonSettings commonSettings,
            IAclService aclService,
            ICacheManager cacheManager,
            IDataProvider dataProvider,
            IDbContext dbContext,
            IEventPublisher eventPublisher,
            ILocalizationService localizationService,
            IRepository<AclRecord> aclRepository,
            IRepository<Company> companyRepository,
            IRepository<StoreMapping> storeMappingRepository,
            IStaticCacheManager staticCacheManager,
            IStoreContext storeContext,
            IStoreMappingService storeMappingService,
            IWorkContext workContext)
        {
            _catalogSettings = catalogSettings;
            _commonSettings = commonSettings;
            _aclService = aclService;
            _cacheManager = cacheManager;
            _dataProvider = dataProvider;
            _dbContext = dbContext;
            _eventPublisher = eventPublisher;
            _localizationService = localizationService;
            _aclRepository = aclRepository;
            _companyRepository = companyRepository;
            _storeMappingRepository = storeMappingRepository;
            _staticCacheManager = staticCacheManager;
            _storeContext = storeContext;
            _storeMappingService = storeMappingService;
            _workContext = workContext;
        }

        #endregion

        public virtual Company GetCompanyById(int companyId)
        {
            if (companyId == 0)
                return null;

            var key = string.Format(NopCatalogDefaults.CompaniesByIdCacheKey, companyId);
            return _cacheManager.Get(key, () => _companyRepository.GetById(companyId));
        }

        public virtual void InsertCompany(Company company)
        {
            if (company == null)
                throw new ArgumentNullException(nameof(company));

            if (company is IEntityForCaching)
                throw new ArgumentException("Cacheable entities are not supported by Entity Framework");

            _companyRepository.Insert(company);

            //cache
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.CompaniesPrefixCacheKey);
            _staticCacheManager.RemoveByPrefix(NopCatalogDefaults.CompaniesPrefixCacheKey);

            //event notification
            _eventPublisher.EntityInserted(company);
        }

        public virtual void UpdateCompany(Company company)
        {
            if (company == null)
                throw new ArgumentNullException(nameof(company));

            if (company is IEntityForCaching)
                throw new ArgumentException("Cacheable entities are not supported by Entity Framework");

            _companyRepository.Update(company);

            //cache
            _cacheManager.RemoveByPrefix(NopCatalogDefaults.CompaniesPrefixCacheKey);
            _staticCacheManager.RemoveByPrefix(NopCatalogDefaults.CompaniesPrefixCacheKey);

            //event notification
            _eventPublisher.EntityUpdated(company);
        }

        public virtual void DeleteCompany(Company company)
        {
            if (company == null)
                throw new ArgumentNullException(nameof(company));

            if (company is IEntityForCaching)
                throw new ArgumentException("Cacheable entities are not supported by Entity Framework");

            company.Deleted = true;
            UpdateCompany(company);

            //event notification
            _eventPublisher.EntityDeleted(company);
        }

        public virtual IList<Company> GetAllCompanies(int storeId = 0, bool showHidden = false, bool loadCacheableCopy = true)
        {
            IList<Company> loadCategoriesFunc() => GetAllCompanies(string.Empty, storeId, showHidden: showHidden);

            IList<Company> companies;
            //if (loadCacheableCopy)
            //{
            //    //cacheable copy
            //    var key = string.Format(NopCatalogDefaults.CompaniesAllCacheKey,
            //        storeId,
            //        string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
            //        showHidden);
            //    companies = _staticCacheManager.Get(key, () =>
            //    {
            //        var result = new List<Company>();
            //        foreach (var company in loadCategoriesFunc())
            //            result.Add(new CategoryForCaching(company));
            //        return result;
            //    });
            //}
            //else
            //{
            companies = loadCategoriesFunc();
            //}

            return companies;
        }

        public virtual IPagedList<Company> GetAllCompanies(string companyName, int storeId = 0, int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            //don't use a stored procedure. Use LINQ
            var query = _companyRepository.Table;
            if (!showHidden)
                query = query.Where(c => c.Published);
            if (!string.IsNullOrWhiteSpace(companyName))
                query = query.Where(c => c.Name.Contains(companyName));
            query = query.Where(c => !c.Deleted);
            query = query.OrderBy(c => c.Id);

            var companies = query.ToList();
            //paging
            return new PagedList<Company>(companies, pageIndex, pageSize);
        }
    }

}