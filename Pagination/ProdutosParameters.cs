namespace APICatalogo.Pagination
{
    public class ProdutosParameters
    {
        const int maxPageSize = 50; //a variable whose value will not change during the lifetime of the program
        public int PageNumber { get; set; } = 1;
        private int _pageSize;
        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = (value > maxPageSize) ? maxPageSize : value; //valor ternário, se o "value" for maior que o "maxPageSize",
                                                                         //retornar "maxPageSize", se o "value" for menor, returnar "value"
            }
        }

    }
}
