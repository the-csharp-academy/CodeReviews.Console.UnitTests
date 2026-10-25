namespace PhoneBook.Application.Categories.UpdateCategory;

public record UpdateCategoryNameRequest(int Id, string OriginalName, string NewName);