namespace N_Tier.Application.Models.TodoItem;

public class CreateTodoItemModel
{
    public int TodoListId { get; set; }

    public string Title { get; set; }

    public string Body { get; set; }
}

public class CreateTodoItemResponseModel : BaseResponseModel { }
