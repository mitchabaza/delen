#Delen
I thought it might be fun to create a simple distributed task runner.  I'm a huge fan of TeamCity, so it's no surpise that I took inspiration from its approach to scheduling & dispatching jobs using a master/slave architecture. Delen (taken from the Dutch word for "share")  is written using C#, MVC & RavenDB for persistence.  

v.0001 has support for generic job scheduling, worker registeration/deregistration, and job execution.  Also, this project is 100% not ready for prime time and should not be used to power any production systems.  

###What's next  
* Support for job parallelization across workers via some type of job chunking/splitting 
* Periodic resource collection from  all active workers to optimize job distribution
* Other cool stuff I will probably never have the time to implement.


