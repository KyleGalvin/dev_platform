# Environment

Environment variables play a key part in the functionality of our microservice applications.

In the kubernetes tilt environment, environment variables are defined in yaml files. Most of them should be collected and centralized in the kubernetes.yml file in a configmap that acts as the master set of environment variables for this environment.

From there, applications that need access to these can import the configmap, remap the names if needed, and use them as needed.

When [debugging](debugging.md), the application is no longer running in the kubernetes environment and thus cannot get the environment variables by the usual method. In these cases, the debug config should supply its own variables. These variables may differ from the ones within the cluster, since the names and ports of the cluster services may change when an application must go through tilt or a gateway application to reach cluster services.