using AutoMapper;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class MappingService : IMappingService
    {
        private readonly IMapper _mapper;

        public MappingService(IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public TDest Map<TSrc, TDest>(TSrc source) where TDest : class
        {
            return _mapper.Map<TSrc, TDest>(source);
        }
    }
}
