using BankBranchManagementSystem.Models;
using BankBranchManagementSystem.Dtos;

namespace BankBranchManagementSystem.Interfaces
{
    public interface IBranchService
    {

        Task<IEnumerable<BranchListDto>> GetAllBranchesAsync();

        Task<Branch?> GetBranchAsync(int id);

        Task AddBranchAsync(Branch branch, int userID);

        Task UpdateBranchAsync(Branch branch, string? oldManagerNewJobT, int userID);

        Task DeleteBranchAsync(int id, int userID);

        Task UpdateBranchStatusAsync(int id, bool isActive, int userID);

        //Task<IEnumerable<Branch>> SearchByNameAsync(string name);

        //Task<IEnumerable<Branch>> SearchByCodeAsync(string code);

        // Task<IEnumerable<Branch>> SearchByCityAsync(string city);

        //Task<IEnumerable<Branch>> FilterAsync(string? city, string? name);

        Task<BranchDetailsDto?> GetBranchDetailsAsync(int id);

        Task<bool> HasActiveEmployeesAsync(int id);

        Task<IEnumerable<BranchListDto>> SearchBranchesAsync(string searchTerm);

        Task<int> GetTotalBranchesAsync();
        Task<int> GetActiveBranchesAsync();
        Task<IEnumerable<Branch>> GetRecentlyAddedBranchesAsync();


    }
}
