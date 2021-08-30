# Searchify Indexer Service


This is a document indexing microservice built as part of school coursework


### Requirements
These requirements should be satisfied before proceeding to other sections of this document:
- [Docker](https://docs.docker.com/desktop/)
- [docker-compose](https://github.com/docker/compose)
- [git](https://git-scm.com/book/en/v2/Getting-Started-Installing-Git)


## Running all services with docker-compose
Included in this folder is a `docker-compose.yml` file that would aid the startup of all services. Run the following command:

```
$ docker-compose /path/to/docker-compose.yml up -d
```

Below is a list of each service and the port they will listen on on your machine when they are all running:
- indexer ~ :5050
- db_store ~ :8000


## Note
In order to persist the data in the database container, we make use of a mounted volume. The volumes maps to a directory on your machine and it may require some permissions in order to run. In case you are running into errors with the containers, you can take these steps:

Windows: run docker-compose commands in an elevated prompt

MacOS or Linux:
```
# change directory to the same folder as your docker-compose.yml file

# create directories for both db volumes
$ mkdir ./data/dynamodb

# give permission to be readable writeable and executable by all users
$ sudo chmod 777 ./data/dynamodb
```

After following these steps, you can execute the docker-compose commands again.
