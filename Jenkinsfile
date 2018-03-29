node {
    stage('checkout') {
        // git 'https://github.com/wuqi618/PipelineSample.git'
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
        dir('src/WebApiSample/WebApiSample/') {
            sh 'rm -rf Publish'
            sh 'dotnet publish WebApiSample.csproj -c Release -r ubuntu.16.04-x64 -o Publish'
            def output = sh returnStdout: true, script: 'aws ecr get-login --region ap-southeast-2'
            output = output.replaceFirst(" -e none ", " ")
            sh "$output"
            sh 'docker build -t webapisample -f Dockerfile.ci .'
            sh "docker tag webapisample:latest 531585151505.dkr.ecr.ap-southeast-2.amazonaws.com/pipeline-ecs-ecr:${BUILD_NUMBER}"
            sh "docker push 531585151505.dkr.ecr.ap-southeast-2.amazonaws.com/pipeline-ecs-ecr:${BUILD_NUMBER}"
        }
    }

    stage('deploy') {
        dir('CloudFormation/') {
            def dir = sh returnStdout: true, script: 'echo $PWD'
            def image = "531585151505.dkr.ecr.ap-southeast-2.amazonaws.com/pipeline-ecs-ecr:${BUILD_NUMBER}"
            def cluster = "pipeline-ecs-cluster"
            sh "aws cloudformation create-stack --stack-name webapisample --template-body file://${dir}/ecs.yml --parameters Image=${image},ECSCluster=${cluster}"
        }
    }
    
    // stage('zip & s3') {
    //     dir('src/WebApiSample/WebApiSample/') {
    //         sh 'rm -f publish.zip'
    //         sh 'zip -r publish.zip Publish'
    //         sh "aws s3 cp publish.zip s3://webapisample-publish/publish-${BUILD_NUMBER}.zip"
    //     }
    // }
    
    // stage('archive') {
    //     dir('src/WebApiSample/Tests/') {
    //         step([$class: 'XUnitPublisher', testTimeMargin: '3000', thresholdMode: 1, thresholds: [[$class: 'FailedThreshold', failureNewThreshold: '0', failureThreshold: '0', unstableNewThreshold: '0', unstableThreshold: '0'], [$class: 'SkippedThreshold', failureNewThreshold: '', failureThreshold: '', unstableNewThreshold: '', unstableThreshold: '']], tools: [[$class: 'XUnitDotNetTestType', deleteOutputFiles: true, failIfNotNew: true, pattern: 'TestResult/*.xml', skipNoTestFiles: false, stopProcessingIfError: true]]])
    //     }
        
    //     dir('src/WebApiSample/WebApiSample/Publish/') {
    //         archiveArtifacts '**'
    //     }
    // }
}
