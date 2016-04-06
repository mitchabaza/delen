#Delen
I thought it might be fun to create a simple distributed task runner.  I'm a huge fan of TeamCity, so it's no surpise that I took inspiration from its approach to scheduling & dispatching jobs using a master/slave architecture. Delen (taken from the Dutch word for "share")  is written using C#, MVC & RavenDB for persistence.  

v1.0 has support for generic job scheduling, worker registeration/deregistration, and job execution.  Support for job  paralleization via chunking if I can find the time to do it.  

