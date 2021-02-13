using Hahn.ApplicatonProcess.December2020.Data.DBContext;
using Hahn.ApplicatonProcess.December2020.Data.Entity;
using Hahn.ApplicatonProcess.December2020.Data.Generics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hahn.ApplicatonProcess.December2020.Data.UnitOfWork
{
    public class Uow : IUoW
    {
        private ApplicationDbContext DbContext { get; set; }

        public Uow(ApplicationDbContext _DbContext)
        {
            DbContext = _DbContext;
        }
        public void Commit()
        {
            DbContext.SaveChanges();
        }

        public IRepository<Applicant> ApplicantRepository
        {
            get { return new Repository<Applicant>(DbContext); }
        }

        //public IRepository<User> UserRepository
        //{
        //    get { return new Repository<User>(DbContext); }
        //}

       
    }

    public interface IUoW
    {
        void Commit();

        IRepository<Applicant> ApplicantRepository { get; }
        //IRepository<User> UserRepository { get; }
        

    }
}
