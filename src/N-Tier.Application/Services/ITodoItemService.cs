using N_Tier.Application.Models;
using N_Tier.Application.Models.TodoItem;

namespace N_Tier.Application.Services;

public interface ITodoItemService
{
    Task<CreateTodoItemResponseModel> CreateAsync(CreateTodoItemModel createTodoItemModel,
        CancellationToken cancellationToken = default);

    Task<BaseResponseModel> DeleteAsync(int id, CancellationToken cancellationToken = default);

    Task<IEnumerable<TodoItemResponseModel>>
        GetAllByListIdAsync(int id, CancellationToken cancellationToken = default);

    Task<UpdateTodoItemResponseModel> UpdateAsync(int id, UpdateTodoItemModel updateTodoItemModel,
        CancellationToken cancellationToken = default);
}
