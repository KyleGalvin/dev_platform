# Grafana

[Grafana](https://grafana.com/ "grafana.com") is a dashboarding, observability, and visualization tool meant for graphing and analyzing system data from a collection of sources.

SeaSprig uses this to ingest and view application logs from various microservices. This happens in multiple steps.

First, an application called [promtail](https://grafana.com/docs/loki/latest/send-data/promtail/ "promtail agent") is in charge of looking in the kubernetes cluster and polling all pods for logs. Logs retrieved are then put through a renaming process, where context can be added and logs can be normalized.

From there, promtail will forward the logs to [loki](https://grafana.com/docs/loki/latest/ "grafana loki") which acts as a datastore for the collected logs.

This datastore is then added to grafana as a data source, where users can build custom queries and dashboards around the data.
