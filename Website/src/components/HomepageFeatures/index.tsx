import type {ReactNode} from 'react';
import clsx from 'clsx';
import Heading from '@theme/Heading';
import styles from './styles.module.css';

type FeatureItem = {
  title: string;
  Svg: React.ComponentType<React.ComponentProps<'svg'>>;
  description: ReactNode;
};

const FeatureList: FeatureItem[] = [
  {
    title: '少スペース',
    Svg: require('@site/static/img/layout.svg').default,
    description: (
      <>
        膨大な数のポータルを限られたスペースに収容することができます。
        目的のポータルを探すために、広い空間を歩き回る必要はありません。
      </>
    ),
  },
  {
    title: '軽量',
    Svg: require('@site/static/img/feather.svg').default,
    description: (
      <>
        リストからワールドを選ぶまでポータルを生成しないため、
        大量のポータルを収容してもワールドが重くなりません。
      </>
    ),
  },
  {
    title: '外部JSON読み込み',
    Svg: require('@site/static/img/download.svg').default,
    description: (
      <>
        ワールドのリストを外部からJSONデータの形で読み込むことができます。
        ワールドの再アップロードを行うことなく、ワールドリストの更新が可能です。
      </>
    ),
  },
];

function Feature({title, Svg, description}: FeatureItem) {
  return (
    <div className={clsx('col col--4')}>
      <div className="text--center">
        <Svg className={styles.featureSvg} role="img" />
      </div>
      <div className="text--center padding-horiz--md">
        <Heading as="h3">{title}</Heading>
        <p>{description}</p>
      </div>
    </div>
  );
}

export default function HomepageFeatures(): ReactNode {
  return (
    <section className={styles.features}>
      <div className="container">
        <div className="row">
          {FeatureList.map((props, idx) => (
            <Feature key={idx} {...props} />
          ))}
        </div>
      </div>
    </section>
  );
}
