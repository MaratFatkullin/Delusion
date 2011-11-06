using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AI_.Data;
using AI_.Data.Repository;

namespace AI_.Security.Tests.Mocks
{
    public class RepositoryMock<TEntity>
        : IRepository<TEntity>, IObserver<object>
        where TEntity : ModelBase
    {
        private readonly IList<TEntity> _storage;
        private readonly IList<Command> _commands;

        public IList<TEntity> Storage
        {
            get { return _storage; }
        }

        public RepositoryMock(IObservable<object> unitOfWork)
        {
            unitOfWork.Subscribe(this);
            _commands = new List<Command>();
            _storage = new List<TEntity>();
        }

        #region IRepository<TEntity> Members

        public ICollection<TEntity> Get(Expression<Func<TEntity, bool>> filter = null,
                                        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                        string includeProperties = "")
        {
            IQueryable<TEntity> query = _storage.AsQueryable();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            return query.ToList();
        }

        public TEntity GetByID(object id)
        {
            return _storage.Where(entity => entity.ID == (int) id).FirstOrDefault();
        }

        public void Insert(TEntity entity)
        {
           _commands.Add(new Command(entity,CommnadType.Insert));
        }

        public void Delete(object id)
        {
            var entityToDelete = _storage.Single(entity => entity.ID == (int) id);
            new Command(entityToDelete, CommnadType.Delete);
        }

        public void Delete(TEntity entityToDelete)
        {
            Delete(entityToDelete.ID);
        }

        public void Update(TEntity entityToUpdate)
        {
            _commands.Add(new Command(entityToUpdate,CommnadType.Update));
        }

        #endregion

        public void OnNext(object value)
        {
            foreach (var command in _commands)
            {
                command.Execute(_storage);
            }
            _commands.Clear();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }
    }

    internal class Command
    {
        public ModelBase Argument { get; private set; }
        public CommnadType Type { get; private set; }

        public Command(ModelBase argument, CommnadType type)
        {
            Argument = argument;
            Type = type;
        }

        public void Execute<TEntity>(IList<TEntity> storage) where TEntity : ModelBase
        {
            var enityt = Argument as TEntity;
            switch (Type)
            {
                case CommnadType.Insert:
                    Argument.ID = storage.Count + 1;
                    Argument.CreateDate = DateTime.Now;
                    storage.Add(enityt);
                    break ;
                case CommnadType.Update:
                    var item = storage.First(entity => entity.ID == Argument.ID);
                    Argument.CreateDate = item.CreateDate;
                    Argument.UpdateDate = DateTime.Now;
                    storage.Remove(item);
                    storage.Add(enityt);
                    break;
                case CommnadType.Delete:
                    storage.Remove(enityt);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    internal enum CommnadType
    {
        Insert, Update, Delete
    }
}
