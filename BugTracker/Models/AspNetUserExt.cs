﻿using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
	public partial class AspNetUserExt : IdentityUser
	{
        enum Permissions { ProjectManager, Developer }
	}
}