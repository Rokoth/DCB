using System.Collections.Generic;

namespace DCB.DB.Model
{
    public class PagedResult<T>
    {
        public PagedResult(IEnumerable<T> data, int allCount)
        {
            Data = data;
            AllCount = allCount;
        }
        public IEnumerable<T> Data { get; }
        public int AllCount { get; }
    }
}