# FRINDOC
FRINDOC aims at providing a comprehensive overview of good practices and valuable experiences for universities. The project will develop a framework containing a statement of good practice on internationalisation and an online tool for universities to aid planning and implementation of internationalisation strategies for doctoral education. It will be a comprehensive strategic tool for planning, promoting and supporting mobility in doctoral programmes enabling universities to attain a united picture of strategic goals, capacity and possibilities to implement the right structures for their particular profile.

## Getting Started
Start by opening the solution in Visual Studio. The project uses Code-First Entity Framework which means that you can let Entity Framework generate the database structure for you. To do this, follow these steps:

1. Setup a database for this project in SQL Server Management Studio
2. Set the proper connectionstring for "DefaultConnection" in the Web.config of the Frindoc.Web project
3. Inside Visual Studio open the "Package Manager Console"
4. Run the command `Update-Database`

This will populate your database with the proper database structure.

Now you're ready to run the project.

## Frameworks being used
### KnockoutJs (http://knockoutjs.com)
By encapsulating data and behavior into a view model, you get a clean, extensible foundation on which to build sophisticated UIs without getting lost in a tangle of event handlers and manual DOM updates.
### Bootstrap (http://getbootstrap.com)
Bootstrap is the most popular HTML, CSS, and JS framework for developing responsive, mobile first projects on the web.
### EntityFramework.6.1.3 using code-first approach
Entity Framework is Microsoft's recommended data access technology for new applications.
### Asp.Net MVC 5
ASP.NET MVC is a web framework that gives you a powerful, patterns-based way to build dynamic websites and Web APIs. ASP.NET MVC enables a clean separation of concerns and gives you full control over markup.

