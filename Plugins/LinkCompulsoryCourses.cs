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
                // Obtain the execution context from the service provider.  
                IPluginExecutionContext context = (IPluginExecutionContext)localContext.PluginExecutionContext;

                // Obtain the organization service reference for web service calls.  
                IOrganizationService currentUserService = localContext.CurrentUserService;

                // Implement your custom Plug-in business logic.
                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    // Obtain the target entity from the input parameters.
                    Entity student = (Entity)context.InputParameters["Target"];

                    // Verify that the target entity represents a student.
                    if (student.LogicalName != "in23gl_student")
                        return;

                    // Get the university of the student
                    EntityReference studentUniversity = (EntityReference)student["_in23gl_university_value"];

                    // Create a query to retrieve all compulsory courses of the same university
                    // https://learn.microsoft.com/en-us/dotnet/api/microsoft.xrm.sdk.query.querybyattribute?view=dataverse-sdk-latest
                    var query = new QueryByAttribute("in23gl_course")
                    {
                        ColumnSet = new ColumnSet("in23gl_courseid"),
                        Attributes = { "in23gl_iscompulsory", "in23gl_university" },
                        //question: why is the studentUniversity.Id not studentUniversity["in23gl_universityid"]?
                        Values = { true, new EntityReference("in23gl_university", studentUniversity.Id) }
                    };


                    // Execute the query
                    EntityCollection compulsoryCourses = currentUserService.RetrieveMultiple(query);
                // https://learn.microsoft.com/en-us/dotnet/api/microsoft.xrm.sdk.iorganizationservice.associate?view=dataverse-sdk-latest
                    EntityReferenceCollection relatedEntities = new EntityReferenceCollection();

                    // Link student with these courses

                    foreach (Entity course in compulsoryCourses.Entities)
                    {
                        //question: why is the course.Id not course["in23gl_courseid"]?
                        relatedEntities.Add(new EntityReference("in23gl_course", course.Id));
                    }
                    //question: why is the student.Id not student["in23gl_studentid"]?
                    currentUserService.Associate("in23gl_student", student.Id, new Relationship("in23gl_rel_Course_Student"), relatedEntities);

                }

            }
            // Only throw an InvalidPluginExecutionException. Please Refer https://go.microsoft.com/fwlink/?linkid=2153829.
            catch (Exception ex)
            {
                tracingService?.Trace("An error occurred executing Plugin CFA_Plugins.Plugins.LinkCompulsoryCourses : {0}", ex.ToString());
                throw new InvalidPluginExecutionException("An error occurred executing Plugin CFA_Plugins.Plugins.LinkCompulsoryCourses .", ex);
            }
        }
    }
}
