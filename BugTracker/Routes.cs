using System.Web.Routing;
using RestfulRouting;
using BugTracker.Controllers;

[assembly: WebActivator.PreApplicationStartMethod(typeof(BugTracker.Routes), "Start")]

namespace BugTracker
{
    public class Routes : RouteSet
    {
        public override void Map(IMapper map)
        {
            map.DebugRoute("routedebug");

			map.Root<HomeController>(x => x.Index());

			map.Resources<AccountController>(accounts =>
			{
				accounts.As("users");
				accounts.IdParameter("username");
				accounts.Only("Show", "Edit", "Update");

				accounts.Resources<ProjectsController>(projects =>
				{
					projects.Resources<TicketsController>(tickets =>
					{
						tickets.Resources<TicketCommentsController>();
					});
				});
			});

			//map.Resources<ProjectsController>(projects => projects.Resources<TicketsController>());

            /*
             * TODO: Add your routes here.
             * 
            map.Root<HomeController>(x => x.Index());
            
            map.Resources<BlogsController>(blogs =>
            {
                blogs.As("weblogs");
                blogs.Only("index", "show");
                blogs.Collection(x => x.Get("latest"));

                blogs.Resources<PostsController>(posts =>
                {
                    posts.Except("create", "update", "destroy");
                    posts.Resources<CommentsController>(c => c.Except("destroy"));
                });
            });

            map.Area<Controllers.Admin.BlogsController>("admin", admin =>
            {
                admin.Resources<Controllers.Admin.BlogsController>();
                admin.Resources<Controllers.Admin.PostsController>();
            });
             */
        }

        public static void Start()
        {
            var routes = RouteTable.Routes;
            routes.MapRoutes<Routes>();
        }
    }
}