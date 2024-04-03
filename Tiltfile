#build docker images for deployment
docker_build('client', './typescript/client')
docker_build('gateway', './typescript/gateway')
docker_build('quizbuilder', './dotnet', dockerfile='./dotnet/QuizBuilder/Dockerfile', ignore=['.vs/'])
docker_build('installationjob', './dotnet', dockerfile='./dotnet/InstallationJob/Dockerfile', ignore=['.vs/'])
docker_build('mkdocs', './mkdocs')

k8s_yaml(helm('./helm/seasprig', values=['./values.yaml']))

#bind ports from inside the cluster to localhost, and apply labels to organize the tilt UI
k8s_resource('installationjob', labels=["applications"])
k8s_resource('quizbuilder', labels=["applications"])
k8s_resource('postgres', port_forwards=['5432:5432'], labels=["services"])
k8s_resource('mkdocs', labels=["services"])
k8s_resource('client', labels=["applications"])
k8s_resource('gateway', labels=["applications"])
k8s_resource('chart-traefik', port_forwards=['8087:9000', '80:8000', '443:8443'], labels=["networking"])
k8s_resource('chart-loki', port_forwards='3000:3000', labels=["logs"])
k8s_resource('chart-promtail', labels=["logs"])
k8s_resource('chart-grafana', labels=["logs"])
k8s_resource('chart-keycloak', labels=["services"])
k8s_resource('chart-gitea', port_forwards=['22:22'], labels=["cicd"])
k8s_resource('chart-drone', labels=["cicd"])
