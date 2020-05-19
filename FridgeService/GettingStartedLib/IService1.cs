using System;
using System.ServiceModel;

namespace GettingStartedLib
{
    [ServiceContract(Namespace = "http://Microsoft.ServiceModel.Samples")]
    public interface IFridge
    {
        [OperationContract]
        int AddFruit(string fruitType, int count);
        [OperationContract]
        int RemoveFruit(string fruitType, int count);
        [OperationContract]
        int TotalFruit();
    }
}