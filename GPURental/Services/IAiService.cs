using GPURental.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GPURental.Services
{
    public interface IAiService
    {
        Task<int> SuggestPriceAsync(string gpuModel, int vram, List<GpuListing> existingListings);
        Task<List<string>> GetGpuSuggestionsAsync(string userTask, List<GpuListing> availableListings);
    }
}