# **Siolo.NET**

## Simple realization of **SIEM** (Security information and event management system).

---
# **Introduction**

### Siolo.NET is a client-server application that allows you to simulate the real work of SIEM systems.

### In this system, each host is represented as a kind of file storage, we check each download and register the corresponding reports on them thanks to the detection and search systems for possible routes of this file through our system.

---

# **Technology stack**

* ### ASP.NET Core WebAPI
* ### MongoDB
* ### ElasticStack (ElasticSearch, Logstash, Kibana)
* ### Redis
* ### PostgreSQL
* ### Neo4j
* ### VirusTotal API
* ### Docker
* ### Python
---

# **Installation**

## Version #1. IIS Express
### In root directory run  
```dotnet
dotnet build 
```
### Then write  
```
dotnet run --project ./Siolo.NET/Siolo.NET.csproj
```

## OR

### **You can open it in Visual Studio, build and run**

## Version #2 Docker
### Clone this repo, go to main directory, open your favourite command line and type:  

```docker
docker build -t IMAGENAME -f "Siolo.NET/Dockerfile" .
```

```docker
docker run -d -p 8080:80 --name CONTAINERNAME IMAGENAME
```

---

# **Usage**

### Simple example of use is in ./Siolo.PyClient/test_script.txt

## Commands you can use in client:
* ### **help** - **print help information**
* ### **show <what>** - **show smth (sessions, incs, active_hosts)**
* ### **forceuse <ip>** - **use not opened session in current client**
* ### **use <ip>** - **use opened session in current client**
* ### **login_sn <subnet_ip>** - **login subnet to system (can be used for adding persisting hosts)**
* ### **login <ip>** - **login ip to system and open session (it will register ip for first time)**
* ### **forcelogout <ip>** - **force logut ip from system**
* ### **logout <ip/all/cur>** - **logout ip from system**
* ### **report <what> <args>** - **print details about smth (inc inc_id, file hash)**

## In session commands:
* ### **background** - **background current session**
* ### **drop <file_path>** - **trigger drop file event**

## Admin commands
* ### **admin <what> <args>**
* ### **<what>**
    * ### **wildcard <wildcard> <info>** - **create new policy**
    * ### **attach  <ip> <wildcard>** - **attach wildcard to ip**
    * ### **link <ip_1> <ip_2>** - **make specific relation between hosts (for subnets)**

* ### **exit** - **disconnect all sessions and leave script**

___
### Made by ***Anrey Ivanov*** and ***Ilya Gubanov***.