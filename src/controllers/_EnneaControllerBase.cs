using entities;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;
using mxcd.core.services;
using mxcd.core.unitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace controllers
{
    //[ApiController]
    [Route("[controller]")]
    [EnableQuery()]
    public abstract class EnneaControllerBase<T, TKey> : ODataController where T : EntityBase<TKey>
    {
        protected readonly IService<T> Service;
        protected readonly IErrorHandler ErrorHandler;
        protected readonly IUnitOfWork UnitOfWork;
        protected EnneaControllerBase(IService<T> service, IUnitOfWork unitOfWork, IErrorHandler errorHandler)
        {
            Service = service;
            ErrorHandler = errorHandler;
            UnitOfWork = unitOfWork;
        }

        IActionResult SendError(string name, Exception ex)
        {
            var text = $"Error on {name} in {this.Request.Path}";
            this.ErrorHandler.Trace(text, ex);
            return BadRequest(text);
        }

        // GET: api/Enneatype
        //[HttpGet]
        public virtual async Task<IActionResult> Get()
        {
            try
            {
                return Ok(await Service.Get());
            }
            catch (Exception oEx)
            {
                return SendError("GET", oEx);
            }
        }

        public abstract IEnumerable<T> GetById(IEnumerable<T> set, TKey key);


        // GET: api/Enneatype/5
        //[HttpGet("{id}", Name = "Get")]
        public virtual async Task<IActionResult> Get([FromODataUri]TKey key)
        {
            try
            {
                //return Ok((await Service.Get()).Where(x => x.Id.Equals(key)));
                var element = GetById(await this.Service.Get(), key);
                //var element = (await this.Service.Get()).Where(x => x.Id == key).AsQueryable();

                //var result = SingleResult.Create(this.Context.Set<Enneatype>().Where(x=>x.Id.Equals(key)));

                //var query1 = await this.Service.Get();
                //var result = query1.Where(x => x.Id.Equals(key));

                //return Ok(result);
                return Ok(SingleResult.Create(element.AsQueryable()));
            }
            catch (Exception oEx)
            {
                return SendError($"GET({key})", oEx);
            }
        }


        // POST: api/Enneatype
        //[HttpPost]
        public virtual async Task<IActionResult> Post(T item)
        {
            try
            {
                IActionResult result;
                if (!ModelState.IsValid)
                {
                    result = BadRequest(ModelState);
                }
                else
                {
                    await Service.Insert(item);
                    await this.UnitOfWork.SaveChanges();

                    return Created(item);
                }

                return result;
            }
            catch (Exception oEx)
            {
                return SendError($"POST [{JsonSerializer.Serialize(item)}]", oEx);
            }
        }

        // PUT: api/Enneatype/5
        //[HttpPut("{id}")]
        public virtual async Task<IActionResult> Put([FromODataUri] TKey key, T item)
        {
            try
            {
                IActionResult result;
                if (!ModelState.IsValid)
                {
                    result = BadRequest(ModelState);
                }
                else
                {
                    if (!key.Equals(item.Id))
                    {
                        result = BadRequest();
                    }
                    else
                    {
                        await Service.Update(item);
                        await UnitOfWork.SaveChanges();
                        result = Updated(item);
                    }
                }

                return result;
            }
            catch (Exception oEx)
            {
                return SendError($"PUT({key})[{JsonSerializer.Serialize(item)}]", oEx);
            }
        }

        public virtual async Task<IActionResult> Patch([FromODataUri] TKey key, Delta<T> item)
        {
            try
            {
                IActionResult result;

                if (!ModelState.IsValid)
                {
                    result = BadRequest(ModelState);
                }
                else
                {
                    var entity = await Service.Find(key);
                    if (entity == null)
                    {
                        result = NotFound();
                    }
                    else
                    {
                        item.Patch(entity);

                        await Service.Update(entity);
                        await UnitOfWork.SaveChanges();
                        result = Updated(item);
                    }
                }

                return result;
            }
            catch (Exception oEx)
            {
                return SendError($"Patch({key})[{JsonSerializer.Serialize(item)}]", oEx);
            }
        }

        // DELETE: api/ApiWithActions/5
        //[HttpDelete("{id}")]
        public virtual async Task<IActionResult> Delete([FromODataUri] TKey key)
        {
            try
            {
                await Service.Remove(key);
                await UnitOfWork.SaveChanges();
                return StatusCode((int)System.Net.HttpStatusCode.NoContent);
            }
            catch (Exception oEx)
            {
                return SendError($"Delete({key})", oEx);
            }
        }
    }
}
