﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GettingStartedClient.ServiceReference1 {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://Microsoft.ServiceModel.Samples", ConfigurationName="ServiceReference1.IFridge")]
    public interface IFridge {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://Microsoft.ServiceModel.Samples/IFridge/AddFruit", ReplyAction="http://Microsoft.ServiceModel.Samples/IFridge/AddFruitResponse")]
        int AddFruit(string fruitType, int count);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://Microsoft.ServiceModel.Samples/IFridge/AddFruit", ReplyAction="http://Microsoft.ServiceModel.Samples/IFridge/AddFruitResponse")]
        System.Threading.Tasks.Task<int> AddFruitAsync(string fruitType, int count);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://Microsoft.ServiceModel.Samples/IFridge/RemoveFruit", ReplyAction="http://Microsoft.ServiceModel.Samples/IFridge/RemoveFruitResponse")]
        int RemoveFruit(string fruitType, int count);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://Microsoft.ServiceModel.Samples/IFridge/RemoveFruit", ReplyAction="http://Microsoft.ServiceModel.Samples/IFridge/RemoveFruitResponse")]
        System.Threading.Tasks.Task<int> RemoveFruitAsync(string fruitType, int count);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://Microsoft.ServiceModel.Samples/IFridge/TotalFruit", ReplyAction="http://Microsoft.ServiceModel.Samples/IFridge/TotalFruitResponse")]
        int TotalFruit();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://Microsoft.ServiceModel.Samples/IFridge/TotalFruit", ReplyAction="http://Microsoft.ServiceModel.Samples/IFridge/TotalFruitResponse")]
        System.Threading.Tasks.Task<int> TotalFruitAsync();
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IFridgeChannel : GettingStartedClient.ServiceReference1.IFridge, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class FridgeClient : System.ServiceModel.ClientBase<GettingStartedClient.ServiceReference1.IFridge>, GettingStartedClient.ServiceReference1.IFridge {
        
        public FridgeClient() {
        }
        
        public FridgeClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public FridgeClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public FridgeClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public FridgeClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public int AddFruit(string fruitType, int count) {
            return base.Channel.AddFruit(fruitType, count);
        }
        
        public System.Threading.Tasks.Task<int> AddFruitAsync(string fruitType, int count) {
            return base.Channel.AddFruitAsync(fruitType, count);
        }
        
        public int RemoveFruit(string fruitType, int count) {
            return base.Channel.RemoveFruit(fruitType, count);
        }
        
        public System.Threading.Tasks.Task<int> RemoveFruitAsync(string fruitType, int count) {
            return base.Channel.RemoveFruitAsync(fruitType, count);
        }
        
        public int TotalFruit() {
            return base.Channel.TotalFruit();
        }
        
        public System.Threading.Tasks.Task<int> TotalFruitAsync() {
            return base.Channel.TotalFruitAsync();
        }
    }
}