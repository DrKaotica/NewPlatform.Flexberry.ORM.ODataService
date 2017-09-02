namespace NewPlatform.Flexberry.ORM.ODataService.Events
{
    /// <summary>
    /// Default implementation of <see cref="IEventHandlerContainer"/>.
    /// </summary>
    /// <seealso cref="IEventHandlerContainer" />
    internal class EventHandlerContainer : IEventHandlerContainer
    {
        /// <summary>
        /// ������� ��� ������ ������ ����� ����������� �������.
        /// </summary>
        public DelegateBeforeGet CallbackBeforeGet { get; set; }

        /// <summary>
        /// ������� ��� ������ ������ ����� ���������� �������.
        /// </summary>
        public DelegateBeforeUpdate CallbackBeforeUpdate { get; set; }

        /// <summary>
        /// ������� ��� ������ ������ ����� ��������� �������.
        /// </summary>
        public DelegateBeforeCreate CallbackBeforeCreate { get; set; }

        /// <summary>
        /// ������� ��� ������ ������ ����� ��������� �������.
        /// </summary>
        public DelegateBeforeDelete CallbackBeforeDelete { get; set; }

        /// <summary>
        /// ������� ��� ������ ������ ����� ����������� ��������.
        /// </summary>
        public DelegateAfterGet CallbackAfterGet { get; set; }

        /// <summary>
        /// ������� ��� ������ ������ ����� ���������� �������.
        /// </summary>
        public DelegateAfterCreate CallbackAfterCreate { get; set; }

        /// <summary>
        /// ������� ��� ������ ������ ����� ���������� �������.
        /// </summary>
        public DelegateAfterUpdate CallbackAfterUpdate { get; set; }

        /// <summary>
        /// ������� ��� ������ ������ ����� �������� �������.
        /// </summary>
        public DelegateAfterDelete CallbackAfterDelete { get; set; }
    }
}