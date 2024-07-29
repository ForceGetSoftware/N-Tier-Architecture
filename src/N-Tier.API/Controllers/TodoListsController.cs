using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using N_Tier.Application.Models;
using N_Tier.Application.Models.TodoItem;
using N_Tier.Application.Models.TodoList;
using N_Tier.Application.Services;

namespace N_Tier.API.Controllers;

[Authorize]
public class TodoListsController : ApiController
{
    private readonly ITodoItemService _todoItemService;
    private readonly ITodoListService _todoListService;

    public TodoListsController(ITodoListService todoListService, ITodoItemService todoItemService)
    {
        _todoListService = todoListService;
        _todoItemService = todoItemService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(ApiResult<IEnumerable<TodoListResponseModel>>.Success(await _todoListService.GetAllAsync()));
    }

    [HttpGet("{id:int}/todoItems")]
    public async Task<IActionResult> GetAllTodoItemsAsync(int id)
    {
        return Ok(ApiResult<IEnumerable<TodoItemResponseModel>>.Success(
            await _todoItemService.GetAllByListIdAsync(id)));
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(CreateTodoListModel createTodoListModel)
    {
        return Ok(ApiResult<CreateTodoListResponseModel>.Success(
            await _todoListService.CreateAsync(createTodoListModel)));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateAsync(int id, UpdateTodoListModel updateTodoListModel)
    {
        return Ok(ApiResult<UpdateTodoListResponseModel>.Success(
            await _todoListService.UpdateAsync(id, updateTodoListModel)));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        return Ok(ApiResult<BaseResponseModel>.Success(await _todoListService.DeleteAsync(id)));
    }
}
