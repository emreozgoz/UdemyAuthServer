using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using UdemyAuthServer.Core.Repository;
using UdemyAuthServer.Core.Services;
using UdemyAuthServer.Core.UnitOfWork;

namespace UdemyAuthServer.Service.Services
{
    public class GenericService<TEntity, TDto> : IGenericService<TEntity, TDto> where TEntity : class where TDto : class
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<TEntity> _genericRepository;
        public GenericService(IUnitOfWork unitOfWork,IGenericRepository<TEntity> genericRepository)
        {
            _unitOfWork = unitOfWork;
            _genericRepository = genericRepository;

        }
        public async Task<Response<TDto>> AddAsync(TDto entity)
        {
            var newEntity = ObjectMapper.mapper.Map<TEntity>(entity);
            await _genericRepository.AddAsync(newEntity);
            await _unitOfWork.CommitAsync();
            var newDto = ObjectMapper.mapper.Map<TDto>(newEntity);
            return Response<TDto>.Succes(newDto, 200);
        }

        public async Task<Response<IEnumerable<TDto>>> GetAllAsync()
        {
            var products = ObjectMapper.mapper.Map<List<TDto>>(await _genericRepository.GetAllAsync());
            return Response<IEnumerable<TDto>>.Succes(products, 200);
        }

        public async Task<Response<TDto>> GetByIdAsync(int id)
        {
            var product =await _genericRepository.GetByIdAsync(id);
            if (product==null)
            {
                Response<TDto>.Failed("ID Bulunamadı", 404, true);
            }
            return Response<TDto>.Succes(ObjectMapper.mapper.Map<TDto>(product), 200);
        }

        public async Task<Response<NoDataDto>> Remove(int id)
        {
            var idCheck =await _genericRepository.GetByIdAsync(id);
            if (idCheck==null)
            {
                Response<NoDataDto>.Failed("ID Bulunamadı", 404, true);
            }
            _genericRepository.Remove(idCheck);
            await _unitOfWork.CommitAsync();
            return Response<NoDataDto>.Succes(204);
        }

        public async Task<Response<NoDataDto>> Update(TDto entity,int id)
        {
            var isHaveEntity = await _genericRepository.GetByIdAsync(id);
            if (isHaveEntity == null)
            {
                Response<NoDataDto>.Failed("ID Bulunamadı", 404, true);
            }
            var updatedEntity = ObjectMapper.mapper.Map<TEntity>(entity);

            _genericRepository.Update(updatedEntity);
            await _unitOfWork.CommitAsync();
            return Response<NoDataDto>.Succes(204);
        }

        public async Task<Response<IEnumerable<TDto>>> Where(Expression<Func<TEntity, bool>> predicate)
        {
            var list = _genericRepository.Where(predicate);
            return Response<IEnumerable<TDto>>.Succes(ObjectMapper.mapper.Map<IEnumerable<TDto>>(await list.ToListAsync()),200);
        }
    }
}
