# How to create your own templates for dotnet new

[Original Post on devblogs.microsoft.com](https://devblogs.microsoft.com/dotnet/how-to-create-your-own-templates-for-dotnet-new)

April 2nd, 2017

You can now create your own templates for dotnet new. Creating and installing
your own templates is an experimental feature at this point, but one that we
are seeing significant interest in and that deserves more of your feedback
before we enable it for broad use with .NET Core 2.0. The version of dotnet new
shipped with the .NET Core 1.0 SDK has a new command line parameter --install.
This is an undocumented feature, and is not currently included in the help
output.

You can try the new template experience if you have the new SDK or Visual
Studio 2017 installed. If you didn’t you can right now.

The goal of this post is to connect with developers who are interested in
creating templates. If you maintain a library or framework project on GitHub,
then you are a great candidate to be a template author. There are lots of other
cases too, where creating templates makes sense. If you can create a sample,
you can create a template. It’s not hard at all.

In the last update for .NET Core, we have updated dotnet new. This new version
of dotnet new is now built on top of the new Template Engine, which is a
library that we are developing. To learn more about how to use dotnet new see
the docs. In this article, we’ll show how to create some custom templates and
then use them from dotnet new.

Over the past several years we have seen a lot of interest in creating custom
templates. We also heard that it’s too difficult to create and maintain
templates with the existing tools. Because of that we wanted to make it easy to
create, maintain and share templates. Let’s dive into the demos, and see how to
create some templates. Everything that we cover here is in a GitHub repository
at https://github.com/sayedihashimi/dotnet-new-samples.

I have a web project which I’d like to create a template out of. The template
project can be found at Sayedha.StarterWeb. This is a modified version of the
mvc template which is availables out of the box. Before you create a template
out of this, let’s run the sample to see what was created. After running dotnet
restore, and dotnet run, you can view the app at http://localhost:5000 (or if
running in Visual Studio it will launch automatically when you run the app).
The following screenshot of this app running on my machine (I’m creating these
samples on a Mac, but you can use any platform).

runapp01

The app will look pretty familiar if you’ve created an app with this template
in Visual Studio. There are also some strings that need to be replaced when you
create a template out of this. For example the namespace is set to
SayedHa.StarterWeb. This should be updated to match the project name created.
Now let’s create a template out of this and then you can start adding the
replacements that are needed.

## How to create a basic template

To create a template from an existing project you will need to add a new file
.template.configtemplate.json. You should place the .template.config folder at
the root of the files which should become the template. For example, in this
case I’m going to add the .template.config directory in the Sayedha.StarterWeb
folder. This is the same folder that contains the .csproj project file. Let’s
take a look at the content of the template.json file.

{
"author": "Sayed Ibrahim Hashimi",
"classifications": [ "Web" ],
"name": "Sayed Starter Web",
"identity": "Sayedha.StarterWeb",        // Unique name for this template
"shortName": "sayedweb",                 // Short name that can be used on the cli
"tags": {
    "language": "C#"                       // Specify that this template is in C#.
},
"sourceName": "Sayedha.StarterWeb",      // Will replace the string 'Sayedha.StarterWeb' with the value provided via -n.
"preferNameDirectory" : "true"
}

The contents of template.json shown above are all pretty straight forward. The
sourceName field is an optional field that you should pay more attention to.
I’ll tell you why." When a user invokes dotnet new and specifies a new project
name, by using --name, the project is created, and the string value for
sourceName will be replaced with the value provided for --name. In the
template.json example above, sourceName is set to Sayedha.StarterWeb. This
enables all instances of that string to be re-written by the user-provided
value specified at the command line, with --name. This string is also used to
subsitute the namespace name in the .cs file for the project. When a new
project is created with the template, these values will be updated. We will
discuss preferNameDirectory later. Let’s try out our template now and see that
in action.

Now that we have created the template, it’s time to test it out. The first
thing to do is to install the template. To do that, execute the command dotnet
new --install <PATH> where <PATH> is the path to the folder containing
.template.config. When that command is executed it will discover any template
files under that path and then populate the list of available templates. The
output of running that command on my machine is below. In the output you can
see that the sayedweb template appears.

$ dotnet new --install /Users/sayedhashimi/Documents/mycode/dotnet-new-samples/01-basic-template/SayedHa.StarterWeb

    Templates                                 Short Name      Language      Tags
    Console Application                       console         [C#], F#      Common/Console
    Class library                             classlib        [C#], F#      Common/Library
    Unit Test Project                         mstest          [C#], F#      Test/MSTest
    xUnit Test Project                        xunit           [C#], F#      Test/xUnit
    Sayed Starter Web                         sayedweb        [C#]          Web
    Empty ASP.NET Core Web Application        web             [C#]          Web/Empty
    MVC ASP.NET Core Web Application          mvc             [C#], F#      Web/MVC
    Web API ASP.NET Core Web Application      webapi          [C#]          Web/WebAPI
    Solution File                             sln                           Solution


    Examples:
    dotnet new mvc --auth None --framework netcoreapp1.0
    dotnet new sln
    dotnet new --help

Here you can see that the new template is included in the template list as
expected. Before moving on to create a new project using this template, there
are a few important things to mention about this release. After running
install, to reset your templates back to the default list you can run the
command dotnet new --debug:reinit. We don’t currently have support for
--uninstall, but we are working on that. Now let’s move on to using this
template.

To create a new project you can run the following command.

$ dotnet new sayedweb -n Contoso.Web -o Contoso.Web
Content generation time: 150.1564 ms
The template "Sayed Starter Web" created successfully.

After executing this command, the project was created in a new folder named
Contoso.Web. In addition, all the namespace elements in the .cs files have been
updated to be namespace Contoso.Web instead of namespace SayedHa.StarterWeb. If
you recall from the previous screenshot there were two things that needed to be
updated in the app: the title and the copyright. Let’s see how you can add
these parameters to the template.

## How to create a template with replaceable parameters

Now that you have created a basic template, let’s see how you can customize
this a bit by adding parameters. There are two elements in the home page that
should be updated when the template is used.

  • Title
  • Copyright

For each of these, you will create a parameter that can be customized by the
user during project creation. To make these changes the only file that you will
need to modify is the template.json file. The following snippet contains the
updated template.json file content (source files are located in the
02-add-parameters folder).

{
    "author": "Sayed Ibrahim Hashimi",
    "classifications": [ "Web" ],
    "name": "Sayed Starter Web",
    "identity": "SayedHa.StarterWeb",
    "shortName": "sayedweb",
    "tags": {
        "language": "C#"
    },
    "sourceName": "SayedHa.StarterWeb",
    "symbols":{
        "copyrightName": {
            "type": "parameter",
            "defaultValue": "John Smith",
            "replaces":"Sayed Ibrahim Hashimi"
        },
        "title": {
            "type": "parameter",
            "defaultValue": "Hello Web",
            "replaces":"Sayed Web"
        }
    }
}

Here you have added a new element symbols with two child elements, one for each
parameter. Let’s look at the copyrightName element a bit closer. When creating
a parameter, the type value will be parameter. The replaces element defines the
text which will be replaced. In this case Sayed Ibrahim Hashimi will be
replaced. If the user doesn’t pass in a value when invoking this template, the
defaultValue value will be applied to that. In this case, the default is John
Smith.

Now that you’ve added the two parameters you need, let’s test it with dotnet
new. Since you changed the template.json file, you will need to re-invoke
dotnet new -i again to update the template metadata. After installing the
template again, let’s see what the help output looks like. After executing
dotnet new sayedweb -h, in addition to the default help output you see the
following.

    Sayed Starter Web (C#)
    Author: Sayed Ibrahim Hashimi
    Options:
    -c|--copyrightName
    string - Optional
    Default: John Smith

    -t|--title
    string - Optional
    Default: Hello Web

Here you can see the two parameters which you defined in template.json. The
following is an example of invoking this template and customizing these values.

$ dotnet new sayedweb -n Contosog.Web -o Contoso.Web -c Contoso -t ContosoAdmin

This will result in the _Layout.cshtml file being updated. The <title> and
<footer> elements are both updated.

<title>@ViewData["Title"] - Contoso</title>

<footer>
    <p>&copy; 2017 - Contoso</p>
</footer>

Now that you’ve shown how to add a parameter which replaces some text content
in the source project, let’s move to a more interesting example by adding
optional content.

## Add optional content

The existing template that you have created has a few pages, including a
Contact page. Our next step is to make the Contact page an optional part of the
template. The Contact page is integrated into the project in the following
ways.

  • Method in Controllers/HomeController.cs
  • View in Views/Home/Contact.cshtml
  • Link in Views/Home/Shared/_Layout.cshtml

Before you start modifying the sources the first thing you should do is to
create a new parameter, EnableContactPage, in the template.json file. In the
following snippet you can see what needs to be added for this new parameter.

"EnableContactPage":{
    "type": "parameter",
    "dataType":"bool",
    "defaultValue": "false"
}

Here you used "dataType":"bool" to indicate that this parameter should support
true/false values. Now you will use the value of this parameter to determine if
content will be added to the project. First let’s see how you can exclude
Contact.cshtml when EnableContactPage is set to false. To exclude a file from
being processed during creation, you need to add a new element into the
template.json file. The required content to add is.

"sources": [
    {
        "modifiers": [
            {
                "condition": "(!EnableContactPage)",
                "exclude": [ "Views/Home/Contact.cshtml" ]
            }
        ]
    }
]

Here you’ve added a modifier to the sources element which excludes the Views/
Home/Contact.cshtml if EnableContactPage is false. The expression used in the
condition here, (EnableContentPage), is very basic but, you can create more
complex conditions using operators such as &&,||,!,<,>=,etc. For more info see
https://aka.ms/dotnetnew-template-config. Now let’s see how you can modify the
controller and the layout page to conditionally omit the Contact specific
content.

Below is the modified version of HomeController.cs file that contains the
condition for the Contact method.

The following code block contains the contents of HomeController.cs. This shows
how you can make the Contact method conditional based on the value of
EnableContactPage.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SayedHa.StarterWeb.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

#if (EnableContactPage)
        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

#endif
        public IActionResult Error()
        {
            return View();
        }
    }
}

Here you use a C# #if preprocessor directive to define an optional section in
the template. When editing template source files, the idea is that the files
should be editable in a way that allows the files to still be "runnable". For
example, in this case, instead of modifying the C# file by adding elements
which are invalid, the #if/#endif directives are used for template regions.
Because of this each file type has its own syntax for conditional regions. For
more info on what syntax is used for each file type see https://aka.ms/
dotnetnew-template-config.

When this template is processed, if EnableContactPage is true then the Contact
controller will be present in the HomeController.cs file. Otherwise it will not
be present. In addition to the Contact method in the Controller, there is a
link in the _Layout.cshtml file which be omitted if the Contact page is not
created. The code fragment shows the definition of the navbar from the
_Layout.cshtml file.

    <div class="navbar-collapse collapse">
        <ul class="nav navbar-nav">
            <li><a asp-area="" asp-controller="Home" asp-action="Index">Home</a></li>
            <li><a asp-area="" asp-controller="Home" asp-action="About">About</a></li>
@*#if (EnableContactPage)
            <li><a asp-area="" asp-controller="Home" asp-action="Contact">Contact</a></li>
#endif*@
        </ul>
    </div>

Here the tag helper element creating the Contact link is surrounded with a
condition to check the value of EnableContactPage. Similar to the controller,
if EnableContactPage is false then this link (along with #if/#endif lines) will
not be present in the generated _Layout.cshtml file. The full config file is
available at template.confighttps://github.com/sayedihashimi/dotnet-new-samples
/blob/master/03-optional-page/SayedHa.StarterWeb/.template.config/
template.json. Let’s move on to the next example, giving the user a set of
choices.

## Add a choice from a list of options

In the project the background color is set in the site.css, and site.min.css,
to skyblue. You now want to create a new parameter for the template and give
the user a choice of different background colors to choose from. To do this you
will create a new template parameter, and define the available choices in the
template.json file. The parameter name that you are going to create is
BackgroundColor. Here is the snippet to create this new parameter.

"BackgroundColor":{
  "type":"parameter",
  "datatype": "choice",
  "defaultValue":"aliceblue",
  "choices": [
    {
      "choice": "aliceblue",
      "description": "Alice Blue"
    },
    {
      "choice": "dimgray",
      "description":"dimgray"
    },
    {
      "choice":"skyblue",
      "description":"skyblue"
    }
  ],
  "replaces":"skyblue"
}

Here we define the name of the parameter, the available choices, the default
value (aliceblue), and the string that it replaces ‘skyblue’.

## How to create projects with the name matching the directory

Earlier we saw a property in the template.json file, preferNameDirectory, which
we skipped over. This flag helps simplify creating projects where the name of
the project matches the folder name. Most project templates should have this
parameter set to true.

For example, earlier you created a project with the command dotnet new sayedweb
-n Contoso.Web -o Contoso.Web. This created a new project named Contoso.Web in
a folder with the same name. This can be simplified by adding
"preferNameDirectory":"true" in the template.json file. When a project is
created using a template that has this set to true the project name will match
the directory name (assuming that the --name parameter is not passed in). With
this approach, instead of calling dotnet new with both -n and -o it can be
simplified to the commands shown in the following code block.

    $ mkdir Contoso.Web
    $ cd Contoso.Web
    $ dotnet new sayedweb

When the project is created the name of the folder, Contoso.Web will be used as
the project name, and it will be generated into the current directory.

## Closing

In this post, we have shown how you can get started with creating your own
custom templates for dotnet new. We are still working on enabling the end user
scenarios where templates are acquired and used. In this release, the --install
switch is hidden because it’s currently in preview. The syntax of this command
is likely to change. After installing templates, to reset your templates back
to the default list you can run the command dotnet new --debug:reinit In the
following section, you’ll find some links to existing resources. Please share
your comments here and file issues as needed. You can also reach me on twitter
@SayedIHashimi We’re very excited to see the awesome templates that the
community creates. In addition to this blog, we may post dotnet new related
posts to the .NET Web Developer Blog.

## Resources

  • Template Engine repository
  • Templates available for dotnet new
  • wiki
  • template.json reference

Avatar

Sayed Hashimi

Senior Program Manager, Visual Studio
