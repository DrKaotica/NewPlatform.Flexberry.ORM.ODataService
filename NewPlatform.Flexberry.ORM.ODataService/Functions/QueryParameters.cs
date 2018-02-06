namespace NewPlatform.Flexberry.ORM.ODataService.Functions
{
    using Controllers;
    using ICSSoft.STORMNET.Business;
    using Model;
    using System;
    using System.Net.Http;
    using System.Web.OData.Query;

    /// <summary>
    /// ����� ��� �������� ���������� ������� OData.
    /// </summary>
    public class QueryParameters
    {
        /// <summary>
        /// ������.
        /// </summary>
        public HttpRequestMessage Request { get; set; }

        /// <summary>
        /// ���� �������.
        /// </summary>
        public string RequestBody { get; set; }

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

        private DataObjectController _�ontroller;

        /// <summary>
        /// ������������ ��������� ���� ������� ������, ���������������� ��������� ����� ������ ��������� � EDM-������.
        /// </summary>
        /// <param name="edmEntitySetName">��� ������ ��������� � EDM-������, ��� �������� ��������� �������� ������������� �� ���������.</param>
        /// <returns>���� ������� ������, ��������������� ��������� ����� ������ ��������� � EDM-������.</returns>
        public Type GetDataObjectType(string edmEntitySetName)
        {
            DataObjectEdmModel model = (DataObjectEdmModel)_�ontroller.QueryOptions.Context.Model;
            return model.GetDataObjectType(edmEntitySetName);
        }

        /// <summary>
        /// ������ lcs �� ��������� ���� � ������� OData.
        /// </summary>
        /// <param name="type">��� DataObject.</param>
        /// <returns>���������� lcs.</returns>
        public LoadingCustomizationStruct CreateLcs(Type type, string odataQuery = null)
        {
            HttpRequestMessage request = _�ontroller.Request;
            if (odataQuery != null)
            {
                request = new HttpRequestMessage(HttpMethod.Get, odataQuery);
            }

            _�ontroller.QueryOptions = _�ontroller.CreateODataQueryOptions(type, request);
            _�ontroller.type = type;
            return _�ontroller.CreateLcs();
        }

        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="�ontroller">���������� DataObjectController.</param>
        internal QueryParameters(DataObjectController �ontroller)
        {
            _�ontroller = �ontroller;
            if (�ontroller.QueryOptions == null)
            {
                return;
            }

            if (�ontroller.QueryOptions.Skip != null)
            {
                Skip = �ontroller.QueryOptions.Skip.Value;
            }

            if (�ontroller.QueryOptions.Top != null)
            {
                Top = �ontroller.QueryOptions.Top.Value;
            }
        }
    }
}
