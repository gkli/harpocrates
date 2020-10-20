# harpocrates
Harpocrates is a solution designed to automate the lifecycle of various service and application secrets, their scheduled rotations, as well as updates to dependent secrets that are build based on the value of the managed system secret, like connection strings that may need access key or password to be inserted into a more complex string expression.

Harpocrates relies on Azure Key Vault eventing mechanism to set expirations data and trigger processes based on the events raised by KV. It follows a high level process flow depicted below

![High level process flow](https://raw.githubusercontent.com/gkli/harpocrates/master/Docs/images/bpm3.png)

Harpocrates can be deployed to a number of compute services in Azure, such as AKS, App Service, etc as well as an app that can be deployed to an IaaS VM. Depicted below are high level deployment diagrams for AKS & App Service

Azure Kubernetes Services Deployment
![AKS Deployment](https://raw.githubusercontent.com/gkli/harpocrates/master/Docs/images/deploy-k8.png)

Azure AppService Deployment
![AppService Deployment](https://raw.githubusercontent.com/gkli/harpocrates/master/Docs/images/deploy-appservice.png)
