namespace CoreLibrary.Controller
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;

    using AutoMapper;

    using CoreLibrary.Interfaces;
    using CoreLibrary.Models;
    using CoreLibrary.ViewModels;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Controle base para APIs.
    /// </summary>
    /// <typeparam name="TEntity">
    /// Entidade
    /// </typeparam>
    /// <typeparam name="TViewModel">
    /// ViewModel
    /// </typeparam>
    public class BaseApiController<TEntity, TViewModel> : ControllerBase
        where TEntity : BaseEntity
        where TViewModel : BaseViewModel
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseServiceAsync<TEntity> _serviceAsync;

        /// <summary>
        ///
        /// </summary>
        /// <param name="context"></param>
        /// <param name="serviceAsync"></param>
        /// <param name="mapper"></param>
        public BaseApiController
            (
            IUnitOfWork context,
            IBaseServiceAsync<TEntity> serviceAsync,
            IMapper mapper
            )
        {
            _unitOfWork = context;
            _serviceAsync = serviceAsync;
            _mapper = mapper;
        }

        /// <summary>
        /// Busca todas as entidades.
        /// </summary>
        /// <returns>
        /// Resultado da requisição.
        /// </returns>
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> SearchAll()
        {
            var entityList = await _serviceAsync.GetAllItemsAsync();

            if (!entityList.Any())
                return NotFound("Não foram encontrados itens no banco de dados.");

            var entityListViewModels =
                _mapper.Map<IEnumerable<TEntity>, List<TViewModel>>(entityList);

            return entityListViewModels == null
                ? NotFound("Não foi possível converter os objetos encontrados.")
                : Ok(entityListViewModels);
        }

        /// <summary>
        /// Busca uma entidade específica.
        /// </summary>
        /// <param name="id">Id da entidade.</param>
        /// <returns>
        /// Resultado da requisição.
        /// </returns>
        [HttpGet("{id}", Name = "Get")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> Search(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest();

            var entity = await _serviceAsync.GetItemAsync(id);

            if (entity is null)
                return NotFound();

            var entityViewModel = _mapper.Map<TEntity, TViewModel>(entity);

            return entityViewModel == null
                ? NotFound()
                : Ok(entityViewModel);
        }

        /// <summary>
        /// Atualiza a entidade.
        /// </summary>
        /// <param name="id">Id da entidade.</param>
        /// <param name="entityViewModel">Nova informações da entidade.</param>
        /// <returns>
        /// Resultado da requisição.
        /// </returns>
        [HttpPut("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.UnprocessableEntity)]
        public async Task<IActionResult> Update(Guid id, TViewModel entityViewModel)
        {
            if (id == Guid.Empty)
                return BadRequest();

            if (id != entityViewModel.Id) return BadRequest();

            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var entity = _mapper.Map<TViewModel, TEntity>(entityViewModel);

                    if (entity is null)
                        return UnprocessableEntity();

                    _ = await _serviceAsync.UpdateItem(id, entity);
                    await _unitOfWork.CommitTransactionAsync(transaction);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await EntityExists(id))
                        return NotFound();
                    else
                        throw;
                }
                catch (Exception ex)
                {
                    _unitOfWork.RollbackTransaction();
                    throw new Exception(ex.Message);
                }
            }

            return Ok();
        }

        /// <summary>
        /// Adiciona novo item.
        /// </summary>
        /// <param name="entityViewModel">ViewModel a ser adicionada.</param>
        /// <returns>
        /// Resultado da requisição.
        /// </returns>
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.UnprocessableEntity)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> Add(TViewModel entityViewModel)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    if (await EntityExists(entityViewModel.Id))
                        return Unauthorized();

                    var entity = _mapper.Map<TViewModel, TEntity>(entityViewModel);

                    if (entity is null)
                        return UnprocessableEntity();

                    _ = await _serviceAsync.AddItemAsync(entity);
                    await _unitOfWork.CommitTransactionAsync(transaction);
                }
                catch (Exception ex)
                {
                    _unitOfWork.RollbackTransaction();
                    throw new Exception(ex.Message);
                }
            }

            return Ok(entityViewModel);
        }

        /// <summary>
        /// Remove uma entidade da base de dados.
        /// </summary>
        /// <param name="id">
        /// Id da entidade a ser removida.
        /// </param>
        /// <returns>
        /// Resultado da requisição.
        /// </returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Remove(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest();

            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var entity = await _serviceAsync.GetItemAsync(id);

                    if (entity is null)
                        return NotFound();

                    _serviceAsync.DeleteItem(entity);
                    await _unitOfWork.CommitTransactionAsync(transaction);
                }
                catch (Exception ex)
                {
                    _unitOfWork.RollbackTransaction();
                    throw new Exception(ex.Message);
                }
            }

            return await EntityExists(id)
                ? BadRequest()
                : Ok();
        }

        private async Task<bool> EntityExists(Guid id)
        {
            return (
                await _serviceAsync
                .GetAllItemsAsync()
                .ConfigureAwait(false)
                ).ToList().Any(e => e.Id == id);
        }
    }
}
