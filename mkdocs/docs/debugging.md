# Debugging

When developing microservices, debugging and troubleshooting has an added layer of complexity due to requests spanning multiple processes and services.

While tilt spawns a collection of kubernetes pods that consist of a full set of applications with [logs](grafana.md) gathered in grafana, their operation is otherwise pretty opaque.

When it comes to troubleshooting a single application in isolation, it is best to write tests and step through the application logic step by step in a debugger.

If the issue spans across applications, it is possible to use telepresence to intercept and redirect traffic from a cluster application to a locally running version outside of the cluster that is running in debug mode.

One challenge to this is that the [environment variables](environment.md) that the cluster yaml files feed to the applications are not present outside of the cluster. This means a development copy of the environment variables needs to be constructed and injected into the local applications under observation.

One gotcha I've encountered is the quirks of tilts routing.

the port forwards in the tilt file do not point to the kubernetes services. Instead, they point directly to the pods. This means local applications accessing resources in the cluster do not interact with the kubernetes service definitions at all. This can cause issues because the pods inside the cluster will use the services for pod-to-pod communication. This means misconfigurations in the services (a bad or misconfigured port, for instance) can go unnoticed at development time, and only come up when using the application within the cluster.