// <copyright file="LinkCompulsoryCourses.cs" company="">
// Copyright (c) 2023 All Rights Reserved
// </copyright>
// <author>Gary Larson</author>
// <date>7/20/2023 5:06:20 PM</date>
// <summary>On create event of Student table, link Students to the compulsory courses of their University</summary>
// <auto-generated>
//     This code was generated by a tool.
// </auto-generated>

using System;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;


namespace CFA_Plugins.Plugins
{
    /// <summary>
    /// LinkCompulsoryCourses Plugin.
    /// </summary>    
    public class LinkCompulsoryCourses : PluginBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LinkCompulsoryCourses"/> class.
        /// </summary>
        /// <param name="unsecure">Contains public (unsecured) configuration information.</param>
        /// <param name="secure">Contains non-public (secured) configuration information.</param>
        public LinkCompulsoryCourses(string unsecure, string secure)
            : base(typeof(LinkCompulsoryCourses))
        {

            // TODO: Implement your custom configuration handling.
        }


        /// <summary>
        /// Main entry point for he business logic that the plug-in is to execute.
        /// </summary>
        /// <param name="localContext">The <see cref="LocalPluginContext"/> which contains the
        /// <see cref="IPluginExecutionContext"/>,
        /// <see cref="IOrganizationService"/>
        /// and <see cref="ITracingService"/>
        /// </param>
        /// <remarks>
        /// </remarks>
        protected override void ExecuteCdsPlugin(ILocalPluginContext localContext)
        {
            if (localContext == null)
            {
                throw new InvalidPluginExecutionException(nameof(localContext));
            }
            // Obtain the tracing service
            ITracingService tracingService = localContext.TracingService;

            try
            {
                // Confirm plugin is being executed.
                tracingService.Trace("LinkCompulsoryCourses plugin executed.");
                // Obtain the execution context from the service provider.  
                IPluginExecutionContext context = (IPluginExecutionContext)localContext.PluginExecutionContext;

                // Obtain the organization service reference for web service calls.  
                IOrganizationService currentUserService = localContext.CurrentUserService;

                // Implement your custom Plug-in business logic.

                //Added tracing here to verify inputParameters and logical name
                tracingService.Trace(String.Format("Input parameters: {0}", context.InputParameters));

                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    tracingService.Trace(PluginClassName + " contains Target and is Entity.");
                    // Obtain the target entity from the input parameters.
                    Entity student = (Entity)context.InputParameters["Target"];

                    //Checking if my Target entity is the same. Accidentally registered on Student Log. Verify update to register options with PRT.
                    tracingService.Trace(String.Format("Target entity: {0}", student.LogicalName.ToString()));
                    // Verify that the target entity represents a student.
                    if (student.LogicalName != "in23gl_student")
                        return;

                    //TODO: Check the logical name of the field you are trying to get is correct or not
                    // Get the university of the student. I was able to confirm through FetchXML Builder
                    EntityReference studentUniversity = (EntityReference)student["in23gl_university"];
                    tracingService.Trace(String.Format("Student University ID {0}", studentUniversity.Id.ToString()));
                    tracingService.Trace(String.Format("Student University Logical Name: {0}", studentUniversity.LogicalName.ToString()));
                    // Create a query to retrieve all compulsory courses of the same university
                    // https://learn.microsoft.com/en-us/dotnet/api/microsoft.xrm.sdk.query.querybyattribute?view=dataverse-sdk-latest
                    // Instantiate QueryExpression query
                    var query = new QueryExpression("in23gl_course")
                    {
                        // Add 2 columns to in23gl_course
                        ColumnSet = new ColumnSet("in23gl_coursename", "in23gl_iscompulsory"),
                        // Add filter to in23gl_course with 2 filters
                        Criteria =
                        {
                            // Add 2 filters to in23gl_course
                            Filters =
                            {
                                // Add filter to in23gl_course with 1 conditions
                                new FilterExpression()
                                {
                                    // Add 1 conditions to in23gl_course
                                    Conditions =
                                    {
                                        new ConditionExpression("in23gl_university", ConditionOperator.Equal, studentUniversity.Id)
                                    }
                                },
                                // Add filter to in23gl_course with 1 conditions
                                new FilterExpression()
                                {
                                    // Add 1 conditions to in23gl_course
                                    Conditions =
                                    {
                                        new ConditionExpression("in23gl_iscompulsory", ConditionOperator.Equal, true)
                                    }
                                }
                            }
                        }
                    };


                    // Execute the query
                    EntityCollection compulsoryCourses = currentUserService.RetrieveMultiple(query);
                    // https://learn.microsoft.com/en-us/dotnet/api/microsoft.xrm.sdk.iorganizationservice.associate?view=dataverse-sdk-latest
                    EntityReferenceCollection relatedEntities = new EntityReferenceCollection();

                    // Link student with these courses

                    foreach (Entity course in compulsoryCourses.Entities)
                    {
                        relatedEntities.Add(new EntityReference("in23gl_course", course.Id));
                    }

                    currentUserService.Associate("in23gl_student", student.Id, new Relationship("in23gl_rel_Course_Student"), relatedEntities);

                }

            }
            // Only throw an InvalidPluginExecutionException. Please Refer https://go.microsoft.com/fwlink/?linkid=2153829.
            catch (Exception ex)
            {
                tracingService?.Trace(String.Format(ex.ToString()));
                throw new InvalidPluginExecutionException("An error occurred executing Plugin CFA_Plugins.Plugins.LinkCompulsoryCourses .", ex);
            }
        }
    }
}
