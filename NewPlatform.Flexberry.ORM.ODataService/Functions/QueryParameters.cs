namespace NewPlatform.Flexberry.ORM.ODataService.Functions
{
    using System.Web.OData.Query;

    /// <summary>
    /// ����� ��� �������� ���������� ������� OData.
    /// </summary>
    public class QueryParameters
    {
        /// <summary>
        /// �������� ������� $top.
        /// </summary>
        public int? Top { get; set; }

        /// <summary>
        /// �������� ������� $skip.
        /// </summary>
        public int? Skip { get; set; }

        /// <summary>
        /// ������ ���������� ������������ ��������� � ���������������� �������. ������������ ��� ������������ ����������, ���� � ������� ��� �������� $count=true.
        /// </summary>
        public int? Count { get; set; }

        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="odataOptions">��������� ������� OData.</param>
        internal QueryParameters(ODataQueryOptions odataOptions)
        {
            if (odataOptions.Skip != null)
                Skip = odataOptions.Skip.Value;

            if (odataOptions.Top != null)
                Top = odataOptions.Top.Value;
        }
    }
}
