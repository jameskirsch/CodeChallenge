using CodeChallenge.Models;
using CodeChallenge.Repositories.Interfaces;
using System;
using System.Threading.Tasks;
using System.Transactions;

namespace CodeChallenge.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ICompensationRepository _compensationRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public UnitOfWork(ICompensationRepository compensationRepository, IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
            _compensationRepository = compensationRepository ?? throw new ArgumentNullException(nameof(compensationRepository));
        }

        public IRepository<Employee> Employees { get; }
        public IRepository<Compensation> Compensations { get; }

        public async Task SaveChangesAsync()
        {
            // Use a TransactionScope to ensure both contexts are saved in a single transaction
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            await _employeeRepository.SaveChangesAsync();
            await _compensationRepository.SaveChangesAsync();

            // Complete the transaction
            scope.Complete();
        }
    }
}