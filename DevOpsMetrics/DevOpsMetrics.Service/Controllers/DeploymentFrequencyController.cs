﻿using DevOpsMetrics.Service.DataAccess;
using DevOpsMetrics.Service.Models.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace DevOpsMetrics.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeploymentFrequencyController : ControllerBase
    {
        private readonly IConfiguration Configuration;

        public DeploymentFrequencyController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Get builds from the Azure DevOps API
        /// </summary>
        /// <param name="getSampleData"></param>
        /// <param name="patToken"></param>
        /// <param name="organization"></param>
        /// <param name="project"></param>
        /// <param name="branch"></param>
        /// <param name="buildName"></param>
        /// <param name="buildId"></param>
        /// <param name="numberOfDays"></param>
        /// <param name="maxNumberOfItems"></param>
        /// <param name="useCache"></param>
        /// <returns></returns>
        [HttpGet("GetAzureDevOpsDeploymentFrequency")]
        public async Task<DeploymentFrequencyModel> GetAzureDevOpsDeploymentFrequency(bool getSampleData, string patToken,
            string organization, string project, string branch, string buildName, string buildId,
            int numberOfDays, int maxNumberOfItems, bool useCache)
        {
            DeploymentFrequencyModel model = new DeploymentFrequencyModel();
            try
            {
                TableStorageAuth tableStorageAuth = Common.GenerateTableAuthorization(Configuration);
                DeploymentFrequencyDA da = new DeploymentFrequencyDA();
                model = await da.GetAzureDevOpsDeploymentFrequency(getSampleData, patToken, tableStorageAuth, organization, project, branch, buildName, buildId, numberOfDays, maxNumberOfItems, useCache);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Response status code does not indicate success: 403 (rate limit exceeded).")
                {
                    model.DeploymentName = buildName;
                    model.RateLimitHit = true;
                }
                else
                {
                    throw;
                }
            }
            return model;
        }


        /// <summary>
        /// Get builds from the GitHub API
        /// </summary>
        /// <param name="getSampleData"></param>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        /// <param name="owner"></param>
        /// <param name="repo"></param>
        /// <param name="branch"></param>
        /// <param name="workflowName"></param>
        /// <param name="workflowId"></param>
        /// <param name="numberOfDays"></param>
        /// <param name="maxNumberOfItems"></param>
        /// <param name="useCache"></param>
        /// <returns></returns>
        [HttpGet("GetGitHubDeploymentFrequency")]
        public async Task<DeploymentFrequencyModel> GetGitHubDeploymentFrequency(bool getSampleData, string clientId, string clientSecret,
            string owner, string repo, string branch, string workflowName, string workflowId,
            int numberOfDays, int maxNumberOfItems, bool useCache)
        {
            DeploymentFrequencyModel model = new DeploymentFrequencyModel();
            try
            {
                TableStorageAuth tableStorageAuth = Common.GenerateTableAuthorization(Configuration);
                DeploymentFrequencyDA da = new DeploymentFrequencyDA();
                model = await da.GetGitHubDeploymentFrequency(getSampleData, clientId, clientSecret, tableStorageAuth, owner, repo, branch, workflowName, workflowId, numberOfDays, maxNumberOfItems, useCache);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Response status code does not indicate success: 403 (rate limit exceeded).")
                {
                    model.DeploymentName = workflowName;
                    model.RateLimitHit = true;
                }
                else
                {
                    throw;
                }
            }
            return model;
        }

    }
}