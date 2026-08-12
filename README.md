# GameScheduler Backend

## What is this?

This is a backend API/Dashboard combo for a thesis at Óbuda University. The aim for this project is to be able to host a gameserver (in this case Automobilista 2) according to a given schedule. The servers are all started according to a container orchestrator (i.e.: Kubernetes, Docker Swarm, etc.).

## Requirements
In order to use this software, you will need the following:
- A cluster with a container orchestrator (Kubernetes or Docker Swarm)
- Nodes in the cluster that have at least 4 cores and 8Gb of RAM each
- File storage for logs

## How does it work?
The [docs](./docs) folder has most of the diagrams to know the basics of this program.
