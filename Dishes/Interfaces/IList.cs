using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Dishes.Services;

namespace Dishes.Interfaces
{
    public interface IList
    {
        //public void SetupAdditionalGuiControls();
        //public void InitializeAdditionalGuiData();
        //public void SetupAdditionalEvents();
        //void SetInputFields(IDbEntity obj);
        void FocusPrimaryEditField();
        IDbEntity AddEntity(Service service);
        IDbEntity UpdateEntity(int id, Service service);
        //void ClearAdditionalInputFields();
        List<IDbEntity> GetData(Service service);
        void DeleteEntity(int id, Service service);
        List<IDbEntity> PerformAdditionalFiltering(List<IDbEntity> entities);
        IDataTemplate EditDataTemplate();
        IDbEntity CreateDefaultObject();
        IEnumerable<DataGridColumn> DataGridColumns();
        string RemoveHeader { get; }
        string ConfirmRemoveCaption(int id, Service service);
        void SetupAdditionalEvents(Action updated);
    }
}