namespace Caching.DTOs;

public class GetPagedResponse<T> where T : class
{
    public int Count { get; private set; }
    public int Skip { get; private set; }
    public int Take { get; private set; }
    public int CurrentPage { get; private set; }
    public int TotalPages { get; private set; }
    public IEnumerable<T> Results { get; private set; }

    public GetPagedResponse(int countDataInDatabase, int skip, int take, IEnumerable<T> data)
    {
        Count = countDataInDatabase;
        Skip = skip;
        Take = take;
        CurrentPage = SetCurrentPage(skip, take);
        TotalPages = SetTotalPages(countDataInDatabase, take);
        Results = data;
    }

    private int SetCurrentPage(int skip, int take)
    {
        return skip / take + 1;
    }

    private int SetTotalPages(int countDataInDatabase, int take)
    {
        return
            countDataInDatabase % take != 0 ?
            countDataInDatabase / take + 1 :
            countDataInDatabase / take;
    }
}
