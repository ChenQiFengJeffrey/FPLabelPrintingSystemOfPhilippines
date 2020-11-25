﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace FPLabelPrintingClient.LabelPrintingServiceReference {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="LabelPrintingServiceReference.IPrintSetService")]
    public interface IPrintSetService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPrintSetService/GetPrintSetByFPNum", ReplyAction="http://tempuri.org/IPrintSetService/GetPrintSetByFPNumResponse")]
        System.Data.DataTable GetPrintSetByFPNum(string finishedProductNum);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPrintSetService/GetPrintSetByFPNum", ReplyAction="http://tempuri.org/IPrintSetService/GetPrintSetByFPNumResponse")]
        System.Threading.Tasks.Task<System.Data.DataTable> GetPrintSetByFPNumAsync(string finishedProductNum);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPrintSetService/GetGoodSetByFPNum", ReplyAction="http://tempuri.org/IPrintSetService/GetGoodSetByFPNumResponse")]
        System.Data.DataTable GetGoodSetByFPNum(string finishedProductNum);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPrintSetService/GetGoodSetByFPNum", ReplyAction="http://tempuri.org/IPrintSetService/GetGoodSetByFPNumResponse")]
        System.Threading.Tasks.Task<System.Data.DataTable> GetGoodSetByFPNumAsync(string finishedProductNum);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPrintSetService/GetRoSetByFPNum", ReplyAction="http://tempuri.org/IPrintSetService/GetRoSetByFPNumResponse")]
        System.Data.DataTable GetRoSetByFPNum(string finishedProductNum);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPrintSetService/GetRoSetByFPNum", ReplyAction="http://tempuri.org/IPrintSetService/GetRoSetByFPNumResponse")]
        System.Threading.Tasks.Task<System.Data.DataTable> GetRoSetByFPNumAsync(string finishedProductNum);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPrintSetService/InsertLabelRecord", ReplyAction="http://tempuri.org/IPrintSetService/InsertLabelRecordResponse")]
        void InsertLabelRecord(FPLabelData.FinishedProductLabelDTO[] dtoList);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPrintSetService/InsertLabelRecord", ReplyAction="http://tempuri.org/IPrintSetService/InsertLabelRecordResponse")]
        System.Threading.Tasks.Task InsertLabelRecordAsync(FPLabelData.FinishedProductLabelDTO[] dtoList);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IPrintSetServiceChannel : FPLabelPrintingClient.LabelPrintingServiceReference.IPrintSetService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class PrintSetServiceClient : System.ServiceModel.ClientBase<FPLabelPrintingClient.LabelPrintingServiceReference.IPrintSetService>, FPLabelPrintingClient.LabelPrintingServiceReference.IPrintSetService {
        
        public PrintSetServiceClient() {
        }
        
        public PrintSetServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public PrintSetServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public PrintSetServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public PrintSetServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public System.Data.DataTable GetPrintSetByFPNum(string finishedProductNum) {
            return base.Channel.GetPrintSetByFPNum(finishedProductNum);
        }
        
        public System.Threading.Tasks.Task<System.Data.DataTable> GetPrintSetByFPNumAsync(string finishedProductNum) {
            return base.Channel.GetPrintSetByFPNumAsync(finishedProductNum);
        }
        
        public System.Data.DataTable GetGoodSetByFPNum(string finishedProductNum) {
            return base.Channel.GetGoodSetByFPNum(finishedProductNum);
        }
        
        public System.Threading.Tasks.Task<System.Data.DataTable> GetGoodSetByFPNumAsync(string finishedProductNum) {
            return base.Channel.GetGoodSetByFPNumAsync(finishedProductNum);
        }
        
        public System.Data.DataTable GetRoSetByFPNum(string finishedProductNum) {
            return base.Channel.GetRoSetByFPNum(finishedProductNum);
        }
        
        public System.Threading.Tasks.Task<System.Data.DataTable> GetRoSetByFPNumAsync(string finishedProductNum) {
            return base.Channel.GetRoSetByFPNumAsync(finishedProductNum);
        }
        
        public void InsertLabelRecord(FPLabelData.FinishedProductLabelDTO[] dtoList) {
            base.Channel.InsertLabelRecord(dtoList);
        }
        
        public System.Threading.Tasks.Task InsertLabelRecordAsync(FPLabelData.FinishedProductLabelDTO[] dtoList) {
            return base.Channel.InsertLabelRecordAsync(dtoList);
        }
    }
}
