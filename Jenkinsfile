def config;

node {
    stage('checkout') {
        checkout scm
    }
    
    stage('build') {
       dir('src/WebApiSample/') {
           sh 'dotnet restore -r ubuntu.16.04-x64'
           sh 'dotnet build -c Release -r ubuntu.16.04-x64'
       }
    }
    
    stage('test') {
        dir('src/WebApiSample/Tests/') {
            sh 'dotnet xunit -c Release -xml TestResult/TestResult.xml'
        }
    }
    
    stage('publish') {
        config = getConfig();

        dir('src/WebApiSample/WebApiSample/') {
            publish(config);
        }
    }

    stage('deploy') {
        deploy(config);
    }
    
    // stage('zip & s3') {
    //     dir('src/WebApiSample/WebApiSample/') {
    //         sh 'rm -f publish.zip'
    //         sh 'zip -r publish.zip Publish'
    //         sh "aws s3 cp publish.zip s3://webapisample-publish/publish-${BUILD_NUMBER}.zip"
    //     }
    // }
    
    stage('archive') {
        dir('src/WebApiSample/Tests/') {
            step([$class: 'XUnitPublisher', testTimeMargin: '3000', thresholdMode: 1, thresholds: [[$class: 'FailedThreshold', failureNewThreshold: '0', failureThreshold: '0', unstableNewThreshold: '0', unstableThreshold: '0'], [$class: 'SkippedThreshold', failureNewThreshold: '', failureThreshold: '', unstableNewThreshold: '', unstableThreshold: '']], tools: [[$class: 'XUnitDotNetTestType', deleteOutputFiles: true, failIfNotNew: true, pattern: 'TestResult/*.xml', skipNoTestFiles: false, stopProcessingIfError: true]]])
        }
        
        dir('src/WebApiSample/WebApiSample/Publish/') {
            archiveArtifacts '**'
        }
    }
}

def getConfig() {
    // requires plugin: Pipeline Utility Steps
    def json = readJSON file: "${WORKSPACE}/CloudFormation/ecs-WebApiSample.config";
    json.image = json.image.replaceAll("%BUILD_NUMBER%", BUILD_NUMBER);
    json.template = json.template.replaceAll("%WORKSPACE%", WORKSPACE);
    return json;
}

def publish(config) {
    sh 'rm -rf Publish'
    sh 'dotnet publish WebApiSample.csproj -c Release -r ubuntu.16.04-x64 -o Publish'
    
    def output = sh returnStdout: true, script: "aws ecr get-login --region ${config.region}"
    output = output.replaceFirst(" -e none ", " ")
    sh "${output}"
    sh "docker build -t webapisample -f Dockerfile.ci ."
    sh "docker tag webapisample:latest ${config.image}"
    sh "docker push ${config.image}"
}

def deploy(config) {
    if(stackExists(config)) {
        updateStack(config);
    } else {
        createStack(config);
    }
}

def stackExists(config) {
    def exitValue = sh returnStatus: true, script: "aws cloudformation describe-stacks --stack-name ${config.stackName} --region ${config.region}";
    return exitValue == 0;
}

def createStack(config) {
    sh "aws cloudformation create-stack --stack-name ${config.stackName} --template-body file://${config.template} --parameters ParameterKey=Image,ParameterValue=${config.image} ParameterKey=ECSCluster,ParameterValue=${config.cluster} ParameterKey=VPC,ParameterValue=${config.vpcId} ParameterKey=Subnets,ParameterValue=\\\"${config.subnetIds}\\\" --region ${config.region}";
    sh "aws cloudformation wait stack-create-complete --stack-name ${config.stackName} --region ${config.region}"
}

def updateStack(config) {
    sh "aws cloudformation update-stack --stack-name ${config.stackName} --template-body file://${config.template} --parameters ParameterKey=Image,ParameterValue=${config.image} ParameterKey=ECSCluster,ParameterValue=${config.cluster} ParameterKey=VPC,ParameterValue=${config.vpcId} ParameterKey=Subnets,ParameterValue=\\\"${config.subnetIds}\\\" --region ${config.region}";
    sh "aws cloudformation wait stack-update-complete --stack-name ${config.stackName} --region ${config.region}"
}