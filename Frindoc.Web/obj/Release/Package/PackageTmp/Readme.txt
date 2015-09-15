If you delete the DB file, it still stays registered with SqlLocalDB. Sometimes it fixes it to delete the DB. You can do this from the command line.

Open the "Developer Command Propmpt for VisualStudio" under your start/programs menu.
Run the following commands:

sqllocaldb.exe stop v11.0

sqllocaldb.exe delete v11.0


helpfull links
-----------------
http://code.msdn.microsoft.com/Loop-Reference-handling-in-caaffaf7