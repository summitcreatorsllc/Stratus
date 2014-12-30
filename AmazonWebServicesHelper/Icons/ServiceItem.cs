using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AmazonWebServicesHelper.Icons
{
    public class ServiceItem
    {
        public ServiceItem() { }
        public ServiceItem(string service, string resource, ServiceCategory cat)
        {
            ServiceName = service;
            ResourceName = "pack://application:,,,/AmazonWebServicesHelper;component/Icons/" + cat.ToString() + "/" + resource;
            Category = cat;
        }

        public string ServiceName { get; set; }
        public string ResourceName { get; set; }
        public ServiceCategory Category { get; set; }



        public static List<ServiceItem> GetAllServices()
        {
            List<ServiceItem> services = new List<ServiceItem>();
            // Administration and Security Services
            services.Add(new ServiceItem("CloudTrail",        "cloudtrail/cloudtrailService.png",  ServiceCategory.Administration_Security));
            services.Add(new ServiceItem("CloudWatch",        "cloudwatch/cloudwatchService.png",  ServiceCategory.Administration_Security));
            services.Add(new ServiceItem("Config",            "config/configService.png",          ServiceCategory.Administration_Security));
            services.Add(new ServiceItem("Directory Service", "directory/directoryService.png",    ServiceCategory.Administration_Security));
            services.Add(new ServiceItem("IAM",               "iam/iamService.png",                ServiceCategory.Administration_Security));
            services.Add(new ServiceItem("Trusted Advisor",   "advisor/trustedAdvisorService.png", ServiceCategory.Administration_Security));

            
            // Analytics
            services.Add(new ServiceItem("Data Pipeline",     "datapipeline/datapipelineService.png", ServiceCategory.Analytics));
            services.Add(new ServiceItem("Elastic MapReduce", "mapreduce/mapreduceService.png",       ServiceCategory.Analytics));
            services.Add(new ServiceItem("Kinesis",           "kinesis/kinesisService.png",           ServiceCategory.Analytics));


            // Application
            services.Add(new ServiceItem("AppStream",          "appstream/appstreamService.png",     ServiceCategory.Application));
            services.Add(new ServiceItem("CloudSearch",        "cloudsearch/cloudsearchService.png", ServiceCategory.Application));
            services.Add(new ServiceItem("SES",                "ses/sesService.png",                 ServiceCategory.Application));
            services.Add(new ServiceItem("SQS",                "sqs/sqsService.png",                 ServiceCategory.Application));
            services.Add(new ServiceItem("SWF",                "swf/swfService.png",                 ServiceCategory.Application));
            services.Add(new ServiceItem("Elastic Transcoder", "transcoder/transcoderService.png",   ServiceCategory.Application));


            // Compute
            services.Add(new ServiceItem("EC2",    "ec2/ec2service.png",       ServiceCategory.Compute));
            services.Add(new ServiceItem("Lambda", "lambda/lambdaService.png", ServiceCategory.Compute));


            // Database
            services.Add(new ServiceItem("DynamoDB",    "dynamo/dynamoService.png",           ServiceCategory.Database));
            services.Add(new ServiceItem("ElastiCache", "elasticache/elasticacheService.png", ServiceCategory.Database));
            services.Add(new ServiceItem("RDS",         "rds/rdsService.png",                 ServiceCategory.Database));
            services.Add(new ServiceItem("Redshift",    "rds/rdsService.png",                 ServiceCategory.Database));
            services.Add(new ServiceItem("SimpleDB",    "simpledb/simpledbService.png",       ServiceCategory.Database));

            // Deployment
            services.Add(new ServiceItem("CloudFormation",    "cloudformation/cloudFormationService.png",     ServiceCategory.Deployment_Management));
            services.Add(new ServiceItem("Code Deploy",       "codedeploy/codeDeployService.png",             ServiceCategory.Deployment_Management));
            services.Add(new ServiceItem("Elastic Beanstalk", "elasticbeanstalk/elasticBeanstalkService.png", ServiceCategory.Deployment_Management));
            services.Add(new ServiceItem("OpsWorks",          "opsworks/opsWorksService.png",                 ServiceCategory.Deployment_Management));

            // Enterprise
            services.Add(new ServiceItem("Workspaces", "workspaces/workspacesService.png", ServiceCategory.Enterprise));
            services.Add(new ServiceItem("Zocalo",     "zocalo/zocaloService.png",         ServiceCategory.Enterprise));

            // Mobile
            services.Add(new ServiceItem("Cognito",          "cognito/cognitoService.png",                 ServiceCategory.Mobile));
            services.Add(new ServiceItem("Mobile Analytics", "mobileanalytics/mobileAnalyticsService.png", ServiceCategory.Mobile));
            services.Add(new ServiceItem("SNS",              "sns/snsService.png",                         ServiceCategory.Mobile));


            // Networking
            services.Add(new ServiceItem("Direct Connect", "directConnect/directConnectService.png", ServiceCategory.Networking));
            services.Add(new ServiceItem("Route 53",       "route53/route53service.png",             ServiceCategory.Networking));
            services.Add(new ServiceItem("VPC",            "vpc/vpcService.png",                     ServiceCategory.Networking));


            // Storage
            services.Add(new ServiceItem("Cloudfront",      "cloudfront/cloudfrontService.png",     ServiceCategory.Storage));
            services.Add(new ServiceItem("Storage Gateway", "gateway/storageGatewayService.png",    ServiceCategory.Storage));
            services.Add(new ServiceItem("Glacier",         "glacier/glacierService.png",           ServiceCategory.Storage));
            services.Add(new ServiceItem("Import-Export",   "importexport/importExportService.png", ServiceCategory.Storage));
            services.Add(new ServiceItem("S3",              "s3/s3service.png",                     ServiceCategory.Storage));

            return services;
        }
    }
}
