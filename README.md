Route66
=======

Route66 is a partial implementation of IBM's TN3270 protocol.  It is a web based IBM 3270 terminal emulator written in C#.  It requires the 3270 to provide a Telnet endpoint.

Usage
-----

The back-end service itself is meant to be hosted on your favorite provider that can run .NET applications.  This project includes a Blazor front end that you can use either as is or as a reference implementation of a front end you'd like to build yourself.  Reference implementations in other front end platforms are a work in progress.

The web styling is purposefully very austere.  It is open to styling as you see fit.

The root page of the project launches a simple connection html form that allows you to enter a telnet host address and port to connect to.