using Hero.Shared.DbContext;
using Hero.Shared.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Hero.Shared.Repository
{
    public abstract class ReadOnlyAdoRepository<T> : IReadOnlyRepository<T>
        where T : IAggregateRoot
    {
        protected readonly DbProviderFactory _dbProviderFactory;
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IDBContext _dbContext;

        public ReadOnlyAdoRepository(DbProviderFactory dbProviderFactory, IDBContext dbContext, IUnitOfWork unitOfWork)
        {
            _dbProviderFactory = dbProviderFactory;
            _dbContext = dbContext;
            _unitOfWork = unitOfWork;
        }

        public virtual IEnumerable<T> FindAll()
        {
            throw new NotImplementedException();
        }

        public virtual T FindBy(Guid id)
        {
            throw new NotImplementedException();
        }

        public virtual T PopulateRecord(IDataReader reader)
        {
            throw new NotImplementedException();
        }


        public IEnumerable<T> PopulateRecords(IDataReader reader)
        {
            var records = new List<T>();
            while (reader.Read())
            {
                records.Add(PopulateRecord(reader));
            }
            return records;
        }

        protected IEnumerable<T> GetRecords(string qry, CommandType commandType, params object[] args)
        {
            using (IDbCommand cmd = CreateCommand(qry, commandType, args))
            {
                using (IDataReader reader = cmd.ExecuteReader())
                {
                    return PopulateRecords(reader);
                }
            }

        }

        protected DbCommand CreateCommand(string qry, CommandType commandType, params object[] args)
        {
            DbCommand commmand = _dbContext.CreateCommand();
            commmand.Transaction = (DbTransaction)_unitOfWork.GetTransaction();

            commmand.CommandText = qry;
            commmand.CommandType = commandType;

            FillParameter(commmand, args);

            return commmand;
        }

        protected void FillParameter(DbCommand command, params object[] args)
        {
            if (args == null)
            {
                return;
            }
            int totalColumn = CountColumn(args);
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] is string)
                {
                    FillArrayStringToDbParameter(command, args, totalColumn, ref i);
                }
                else if (args[i] is DbParameter)
                {
                    command.Parameters.Add((DbParameter)args[i]);
                }
                else if (args[i] is Dictionary<string, object>)
                {
                    Dictionary<string, object> dic = args[0] as Dictionary<string, object>;
                    FillDictionaryToDbParameter(command, dic);
                }
                else throw new ArgumentException("invalid agument number.");
            }
        }

        private int CountColumn(object[] args)
        {
            if (args == null || args.Length < 2)
            {
                return 0;
            }

            return args.Length / 2;
        }

        private void FillArrayStringToDbParameter(DbCommand command, object[] args, int totalColumn, ref int index)
        {
            if (index < args.Length - 1)
            {
                DbParameter parm = null;
                if (totalColumn > command.Parameters.Count)
                {
                    parm = _dbProviderFactory.CreateParameter();
                    parm.ParameterName = (string)args[index];
                    FillValueToDBParameter(parm, args[++index]);
                    command.Parameters.Add(parm);
                }
                else
                {
                    parm = command.Parameters[(string)args[index]];
                    FillValueToDBParameter(parm, args[++index]);
                }
            }
        }

        private void FillValueToDBParameter(DbParameter parm, object value)
        {
            if (value == null)
            {
                value = DBNull.Value;
            }
            parm.Value = value;
        }

        private void FillDictionaryToDbParameter(IDbCommand command, Dictionary<string, object> dictionaries)
        {
            if (dictionaries != null)
            {
                foreach (KeyValuePair<string, object> dic in dictionaries)
                {
                    DbParameter parm = _dbProviderFactory.CreateParameter();
                    parm.ParameterName = dic.Key;
                    FillValueToDBParameter(parm, dic.Value);
                    command.Parameters.Add(parm);
                }
            }
        }
    }
}
