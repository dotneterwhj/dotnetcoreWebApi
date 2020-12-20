using DotneterWhj.Core.Attributes;
using DotneterWhj.IRepository;
using DotneterWhj.IServices;
using DotneterWhj.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DotneterWhj.Services
{
    public class AdvertisementService : BaseService<Advertisement>, IAdvertisementService
    {
        public AdvertisementService(IBaseRepository<Advertisement> repository) : base(repository)
        {

        }

        [Caching]
        public async Task Test1()
        {
            return;
        }

        [Caching]
        public Advertisement Test2()
        {
            return new Advertisement();
        }
    }
}
